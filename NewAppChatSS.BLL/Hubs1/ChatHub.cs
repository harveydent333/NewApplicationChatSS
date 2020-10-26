//using AppChatSS.Hubs.CommandHandlersHubs;
//using AppChatSS.Infrastucture;
//using AppChatSS.Infrastucture.ModelHandlers;
//using AppChatSS.Models.Dictionary_Bad_Words;
//using AppChatSS.Models.KickedOuts;
//using AppChatSS.Models.Members;
//using AppChatSS.Models.Messages;
//using AppChatSS.Models.MutedUsers;
//using AppChatSS.Models.Roles;
//using AppChatSS.Models.Rooms;
//using AppChatSS.Models.SwearingUsers;
//using AppChatSS.Models.Users;
//using AppChatSS.Validation;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace NewAppChatSS.BLL.Hubs
//{
//    public class ChatHub1 : Hub
//    {
//        private static IUserRepository userRepository;
//        private static IRoleRepository roleRepository;
//        private static IMessageRepository messageRepository;
//        private static IRoomRepository roomRepository;
//        private static IMemberRepository memberRepository;
//        private static IKickedOutsRepository kickedOutsRepository;
//        private static IMutedUserRepository mutedUserRepository;
//        private static ISwearingUserRepository swearingUserRepository;
//        private static IDictionaryBadWordsRepository dictionaryBadWordsRepository;
//        private static UserValidator userValidator;
//        private static UserCommandHandlerHub commandHandler;
//        private static MessageHandler messageHandler;
//        private static RoomValidator roomValidator;
//        private static RoomHandler roomHundler;

//        public ChatHub(IUserRepository userRep, IRoleRepository roleRep, IRoomRepository roomRep, IMessageRepository messageRep, IMemberRepository memberRep, IKickedOutsRepository kickedOutsRep, IMutedUserRepository mutedUserRep, ISwearingUserRepository swearingUserRep, IDictionaryBadWordsRepository dictionaryBadWordsRep)
//        {
//            userRepository = userRep;
//            roleRepository = roleRep;
//            messageRepository = messageRep;
//            roomRepository = roomRep;
//            memberRepository = memberRep;
//            kickedOutsRepository = kickedOutsRep;
//            mutedUserRepository = mutedUserRep;
//            swearingUserRepository = swearingUserRep;
//            dictionaryBadWordsRepository = dictionaryBadWordsRep;
//            userValidator = new UserValidator(userRepository, memberRepository, kickedOutsRep, mutedUserRep);
//            commandHandler = new UserCommandHandlerHub(userRepository);
//            messageHandler = new MessageHandler(messageRepository, roomRepository, swearingUserRepository, dictionaryBadWordsRepository);
//            roomValidator = new RoomValidator(roomRepository);
//            roomHundler = new RoomHandler(roomRepository, memberRepository);
//        }

//        / <summary>
//        / Метод проверяет заблокирован ли пользователь и имеет ли он возможность отправлять сообщения в чат
//        / </summary>
//        public async Task ReceivingInteractionMessage(String userLogin, String roomId, String message)
//        {
//            User user = userRepository.FindUserByLogin(userLogin);

//            if (UserValidator.IsUserMuted(user.Id, roomId))
//            {
//                await Clients.Caller.SendAsync("ReceiveCommand",
//                    CommandHandler.CreateCommandInfo(String.Format("Вы лишины возможности отправлять сообщения до: {0:U}.",
//                        mutedUserRepository.GetDateTimeUnmutedUser(user.Id, roomId))));
//                return;
//            }

//            if (UserValidator.IsUserBlocked(user))
//            {
//                await Clients.Caller.SendAsync("ReceiveCommand",
//                    CommandHandler.CreateCommandInfo(String.Format("Вы заблокированы до: {0:U}.", user.DateUnblock)));
//                return;
//            }

//            await SendMessage(user, message, roomId, Clients);
//        }

//        / <summary>
//        / Метод обрабатывает принятое сообщение пользователя и вызывает метод клиента, передавая информацию о сообщении для отображения.
//        / </summary>
//        public static async Task SendMessage(User user, String message, String roomId, IHubCallerClients clients)
//        {
//            Room room = roomRepository.Rooms
//             .Where(r => r.Id == roomId)
//             .FirstOrDefault();

//            String messageInfo = messageHandler.SaveMessageIntoDatabase(user, message, room);
//            List<String> members = memberRepository.GetMembers(roomId);

//            await clients.Users(members).SendAsync("ReceiveMessage", messageInfo);
//        }

//        / <summary>
//        / Получение команды взаимодействия с пользователем.
//        / Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя.
//        / Перенаправляет команду в обработчик команд взаимодействия с пользователями.
//        / </summary>
//        public async Task ReceivingUserInteractionCommand(String userLogin, String comamand)
//        {
//            User user = userRepository.FindUserByLogin(userLogin);
//            user.Role = roleRepository.FindRoleById(user.RoleId);

//            await commandHandler.SearchCommand(user, comamand, Clients);
//        }

//        / <summary>
//        /Получение команды взаимодействия с комнатой
//        / Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя, а также комнаты
//        / Перенаправляет команду в обработчик команд взаимодействия с комнатами
//        / </summary>
//        public async Task ReceivingRoomInteractionCommand(String userLogin, String roomId, String command)
//        {
//            User user = userRepository.FindUserByLogin(userLogin);
//            user.Role = roleRepository.FindRoleById(user.RoleId);

//            Room room = roomRepository.FindRoomById(roomId);

//            RoomCommandHandlerHub commandHandler
//                = new RoomCommandHandlerHub(userRepository, roomRepository, memberRepository, kickedOutsRepository, mutedUserRepository);

//            await commandHandler.SearchCommand(user, room, command, Clients);
//        }

//        / <summary>
//        / Метод получает команду взаимодействия с ботом от клиента
//        / Перенаправляет команду в обработчик команд взаимодествия с ботом
//        / </summary>
//        public async Task ReceivingBotInteractionCommand(String comamand)
//        {
//            await BotCommandHandlerHub.SearchCommand(comamand, Clients);
//        }

//        / <summary>
//        / Метод получает команду взаимодействия с ботом от клиента
//        / Перенаправляет команду в обработчик сбора доступных команд для пользователя
//        / </summary>
//        public async Task ReceivingHelpCommand(String userLogin, String roomId, String command)
//        {
//            User user = userRepository.FindUserByLogin(userLogin);
//            user.Role = roleRepository.FindRoleById(user.RoleId);
//            Room room = roomRepository.FindRoomById(roomId);

//            await HelpCommandHandlerHub.SearchCommand(user, room, command, Clients);
//        }
//    }
//}
