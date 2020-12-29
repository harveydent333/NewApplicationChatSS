using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Hubs.CommandHandlersHubs;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Hubs
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

        public ChatHub(
            UserManager<User> userManager,
            IUserValidator userValidator,
            IMessageHandler messageHandler,
            IUserCommandHandler userCommandHandler,
            IRoomCommandHandler roomCommandHandler,
            IUnitOfWork uow,
            IBotCommandHandlerHub botCommandHandler,
            IHelpCommandHandlerHub helpCommandHandler)
        {
            Database = uow;
            this.userManager = userManager;
            this.userValidator = userValidator;
            this.messageHandler = messageHandler;
            this.userCommandHandler = userCommandHandler;
            this.roomCommandHandler = roomCommandHandler;
            this.botCommandHandler = botCommandHandler;
            this.helpCommandHandler = helpCommandHandler;
        }

        public IUnitOfWork Database { get; set; }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь и имеет ли он возможность отправлять сообщения в чат
        /// </summary>
        public async Task ReceivingInteractionMessage(string userName, string roomId, string message)
        {
            User user = await userManager.FindByNameAsync(userName);

            if (await userValidator.IsUserMutedById(user.Id, roomId))
            {
                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo(string.Format(
                        "Вы лишины возможности отправлять сообщения до: {0:U}.", Database.MutedUsers.GetDateTimeUnmuteUser(user.Id, roomId))));
            }

            if (await userValidator.IsUserBlocked(user))
            {
                await Clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo(string.Format("Вы заблокированы до: {0:U}.", user.DateUnblock)));
                return;
            }

            await SendMessage(user, message, roomId, Clients);
        }

        /// <summary>
        /// Метод обрабатывает принятое сообщение пользователя и вызывает метод клиента, передавая информацию о сообщении для отображения.
        /// </summary>
        public async Task SendMessage(User user, string message, string roomId, IHubCallerClients clients)
        {
            Room room = Database.Rooms.GetAll().FirstOrDefault(r => r.Id == roomId);

            string messageInfo = await messageHandler.SaveMessageIntoDatabase(user, message, room);
            List<string> members = Database.Members.GetMembersIds(roomId).ToList();

            await clients.Users(members).SendAsync("ReceiveMessage", messageInfo);
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

            List<string> listLastMessagesIdRoom = Database.Rooms
                .GetAll()
                .Select(r => r.LastMessageId)
                .ToList();

            if (listLastMessagesIdRoom.Contains(messageId))
            {
                Room processedRoom = Database.Rooms
                    .GetAll()
                    .FirstOrDefault(r => r.LastMessageId == messageId);

                await Database.Messages.DeleteMessageAsync(messageId);

                Message proccessedMessage = Database.Messages
                    .GetAll()
                    .OrderByDescending(m => m.DatePublication)
                    .FirstOrDefault(m => m.RoomId == roomId);

                processedRoom.LastMessageId = proccessedMessage.Id;
                await Database.Rooms.UpdateAsync(processedRoom);
                await Clients.Caller.SendAsync("DeleteMessage", messageId);
            }
            else
            {
                await Database.Messages.DeleteMessageAsync(messageId);
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
            User user = await userManager.FindByNameAsync(userName);
            await userCommandHandler.SearchCommandAsync(user, comamand, Clients);
        }

        /// <summary>
        /// Получение команды взаимодействия с комнатой
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя, а также комнаты
        /// Перенаправляет команду в обработчик команд взаимодействия с комнатами
        /// </summary>
        public async Task ReceivingRoomInteractionCommand(string userName, string roomId, string command)
        {
            User user = await userManager.FindByNameAsync(userName);
            Room room = Database.Rooms.FindById(roomId);
            await roomCommandHandler.SearchCommandAsync(user, room, command, Clients);
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
            User user = await userManager.FindByNameAsync(userName);
            Room room = Database.Rooms.FindById(roomId);

            await helpCommandHandler.SearchCommand(user, room, command, Clients);
        }
    }
}
