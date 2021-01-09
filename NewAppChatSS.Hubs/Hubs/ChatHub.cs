using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;
using NewAppChatSS.Hubs.Hubs.CommandHandlersHubs;
using NewAppChatSS.Hubs.Interfaces.HubInterfaces;
using NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;

//TODO: Убрать передачу перменной Client
namespace NewAppChatSS.Hubs.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;
        private readonly IMessageHandler messageHandler;
        private readonly IUserCommandHandler userCommandHandler;
        private readonly IRoomCommandHandler roomCommandHandler;
        private readonly IBotCommandHandlerHub botCommandHandler;
        private readonly IHelpCommandHandlerHub helpCommandHandler;
        private readonly IMessageRepository messageRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IMutedUserRepository mutedUserRepository;
        private readonly IMemberRepository memberRepository;

        public ChatHub(
            IMemberRepository memberRepository,
            IMutedUserRepository mutedUserRepository,
            IMessageRepository messageRepository,
            IRoomRepository roomRepository,
            UserManager<User> userManager,
            IUserValidator userValidator,
            IMessageHandler messageHandler,
            IUserCommandHandler userCommandHandler,
            IRoomCommandHandler roomCommandHandler,
            IBotCommandHandlerHub botCommandHandler,
            IHelpCommandHandlerHub helpCommandHandler)
        {
            this.memberRepository = memberRepository;
            this.mutedUserRepository = mutedUserRepository;
            this.messageRepository = messageRepository;
            this.roomRepository = roomRepository;
            this.userManager = userManager;
            this.userValidator = userValidator;
            this.messageHandler = messageHandler;
            this.userCommandHandler = userCommandHandler;
            this.roomCommandHandler = roomCommandHandler;
            this.botCommandHandler = botCommandHandler;
            this.helpCommandHandler = helpCommandHandler;
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь и имеет ли он возможность отправлять сообщения в чат
        /// </summary>
        public async Task ReceivingInteractionMessage(string userName, string roomId, string message)
        {
            User user = await userManager.FindByNameAsync(userName);

            if (await userValidator.IsUserMutedById(user.Id, roomId))
            {
                var timeUnmuteUser = await mutedUserRepository.GetFirstOrDefaultAsync(new MutedUserModel { UserId = user.Id, RoomId = roomId });

                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo(string.Format(
                        "Вы лишины возможности отправлять сообщения до: {0:U}.", timeUnmuteUser)));
            }

            if (await userValidator.IsUserBlocked(user))
            {
                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo(string.Format("Вы заблокированы до: {0:U}.", user.DateUnblock)));
                return;
            }

            await SendMessage(user, message, roomId);
        }

        /// <summary>
        /// Метод обрабатывает принятое сообщение пользователя и вызывает метод клиента, передавая информацию о сообщении для отображения.
        /// </summary>
        public async Task SendMessage(User user, string message, string roomId)
        {
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            string messageInfo = await messageHandler.SaveMessageIntoDatabase(user, message, room);

            var members = await memberRepository.GetAsync(new MemberModel { RoomId = roomId });
            var memberIds = members.Select(m => m.UserId).ToList();

            await Clients.Users(memberIds).SendAsync("ReceiveMessage", messageInfo);
        }

        // TODO: закомментировать метод
        public async Task DeleteMessage(string userName, string messageId, string roomId)
        {
            User user = await userManager.FindByNameAsync(userName);

            if (user.IsLocked)
            {
                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo("Вы заблокированы и не можете удалять сообщения."));
            }

            var rooms = await roomRepository.GetAsync(new RoomModel { });
            var listLastMessagesIdRoom = rooms.Select(r => r.LastMessageId);

            if (listLastMessagesIdRoom.Contains(messageId))
            {
                var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { LastMessageId = messageId });

                var deleteMessage = await messageRepository.GetFirstOrDefaultAsync(new MessageModel { Ids = new[] { messageId } });
                await messageRepository.DeleteAsync(deleteMessage);

                // TODO: Реализовывать ExtendedModel и фильтровать по убыванию или возврастанию

                var messages = await messageRepository.GetAsync(new MessageModel { });
                var proccessedMessage = messages
                                          .OrderByDescending(m => m.DatePublication)
                                          .FirstOrDefault(m => m.RoomId == roomId);

                processedRoom.LastMessageId = proccessedMessage.Id;

                await roomRepository.ModifyAsync(processedRoom);
                await Clients.Caller.SendAsync("DeleteMessage", messageId);
            }
            else
            {
                var deleteMessage = await messageRepository.GetFirstOrDefaultAsync(new MessageModel { Ids = new[] { messageId } });
                await messageRepository.DeleteAsync(deleteMessage);
                await Clients.Caller.SendAsync("DeleteMessage", messageId);
            }
        }

        /// <summary>
        /// Получение команды взаимодействия с пользователем.
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя.
        /// Перенаправляет команду в обработчик команд взаимодействия с пользователями.
        /// </summary>
        public async Task ReceivingUserInteractionCommand(string userName, string comamand)
        {
            var user = await userManager.FindByNameAsync(userName);
            await userCommandHandler.SearchCommandAsync(user, comamand);
        }

        /// <summary>
        /// Получение команды взаимодействия с комнатой
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя, а также комнаты
        /// Перенаправляет команду в обработчик команд взаимодействия с комнатами
        /// </summary>
        public async Task ReceivingRoomInteractionCommand(string userName, string roomId, string command)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            await roomCommandHandler.SearchCommandAsync(user, room, command);
        }

        /// <summary>
        /// Метод получает команду взаимодействия с ботом от клиента
        /// Перенаправляет команду в обработчик команд взаимодествия с ботом
        /// </summary>
        public async Task ReceivingBotInteractionCommand(string comamand)
        {
            await botCommandHandler.SearchCommand(comamand, Clients);
        }

        /// <summary>
        /// Метод получает команду взаимодействия с ботом от клиента
        /// Перенаправляет команду в обработчик сбора доступных команд для пользователя
        /// </summary>
        public async Task ReceivingHelpCommand(string userName, string roomId, string command)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            await helpCommandHandler.SearchCommand(user, room, command, Clients);
        }
    }
}
