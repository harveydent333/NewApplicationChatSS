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
        private readonly IMessageHub messageHub;

        public ChatHub(
            IMessageHub messageHub,
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
            this.messageHub = messageHub;
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

        // TODO: закомментировать метод
        public async Task DeleteMessageAsync(string userName, string messageId, string roomId)
        {
            User user = await userManager.FindByNameAsync(userName);

            if (user.IsLocked)
            {
                await Clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateResponseMessage("Вы заблокированы и не можете удалять сообщения."));

                return;
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
        /// Метод обрабатывает принятое сообщение пользователя и вызывает метод клиента, передавая информацию о сообщении для отображения.
        /// </summary>
        public async Task SendMessageAsync(User user, string message, string roomId)
        {
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            string messageInfo = await messageHandler.SaveMessageIntoDatabaseAsync(user, message, room);

            var members = await memberRepository.GetAsync(new MemberModel { RoomId = roomId });
            var memberIds = members.Select(m => m.UserId).ToList();

            await Clients.Users(memberIds).SendAsync("ReceiveMessage", messageInfo);
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь и имеет ли он возможность отправлять сообщения в чат
        /// </summary>
        public async Task ReceivingInteractionMessageAsync(string userName, string roomId, string message)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            if (await userValidator.IsUserMutedAsync(user.Id, roomId))
            {
                var timeUnmuteUser = await mutedUserRepository.GetFirstOrDefaultAsync(new MutedUserModel { UserId = user.Id, RoomId = roomId });

                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateResponseMessage(string.Format(
                        "Вы лишины возможности отправлять сообщения до: {0:U}.", timeUnmuteUser)));
                return;
            }

            if (await userValidator.IsUserBlockedAsync(user))
            {
                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateResponseMessage(string.Format("Вы заблокированы до: {0:U}.", user.DateUnblock)));
                return;
            }

            await SendMessageAsync(user, message, roomId);
        }

        /// <summary>
        /// Получение команды взаимодействия с пользователем.
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя.
        /// Перенаправляет команду в обработчик команд взаимодействия с пользователями.
        /// </summary>
        public async Task ReceivingUserInteractionCommandAsync(string userName, string roomId, string comamand)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            await userCommandHandler.SearchCommandAsync(user, room, comamand, Clients);
        }

        /// <summary>
        /// Получение команды взаимодействия с комнатой
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя, а также комнаты
        /// Перенаправляет команду в обработчик команд взаимодействия с комнатами
        /// </summary>
        public async Task ReceivingRoomInteractionCommandAsync(string userName, string roomId, string command)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            await roomCommandHandler.SearchCommandAsync(user, room, command, Clients);
        }

        /// <summary>
        /// Метод получает команду взаимодействия с ботом от клиента
        /// Перенаправляет команду в обработчик команд взаимодествия с ботом
        /// </summary>
        public async Task ReceivingBotInteractionCommandAsync(string userName, string roomId, string command)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            await botCommandHandler.SearchCommandAsync(user, room, command, Clients);
        }

        /// <summary>
        /// Метод получает команду взаимодействия с ботом от клиента
        /// Перенаправляет команду в обработчик сбора доступных команд для пользователя
        /// </summary>
        public async Task ReceivingHelpCommandAsync(string userName, string roomId, string command)
        {
            var user = await userManager.FindByNameAsync(userName);
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            await helpCommandHandler.GetAllowedCommandsAsync(user, room, Clients);
        }
    }
}
