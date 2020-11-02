using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Infrastructure.ModelHandlers;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с комнатой
    /// </summary>
    public class RoomCommandHandlerHub : Hub, IRoomCommandHandler
    {
        private readonly Dictionary<Regex, Func<User, Room, string, IHubCallerClients, Task>> roomCommands;

        const int REGULAR_TYPE_ROOM = 1;
        const int PRITVATE_TYPE_ROOM = 2;
        const int BOT_TYPE_ROOM = 3;

        const string MAIN_ROOM_ID = "1";

        private IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IUserValidator _userValidator;
        private readonly IRoomValidator _roomValidator;
        private readonly IRoomHandler _roomHandler;

        public RoomCommandHandlerHub(UserManager<User> userManager, IUserValidator userValidator, IRoomValidator roomValidator, IUnitOfWork uow, IRoomHandler roomHandler)
        {
            roomCommands = new Dictionary<Regex, Func<User, Room, string, IHubCallerClients, Task>>
            {
                [new Regex(@"^//room\screate\s\w+$")] = CreateRoom,
                [new Regex(@"^//room\screate\s\w+\s-c$")] = CreatePrivateRoom,
                [new Regex(@"^//room\screate\s\w+\s-b$")] = CreateBotRoom,
                [new Regex(@"^//room\sremove\s\w+$")] = RemoveRoom,
                [new Regex(@"^//room\srename\s\w+$")] = RenameRoom,
                [new Regex(@"^//room\sconnect\s\w+\s-l\s\w+$")] = ConnectionToRoom,
                [new Regex(@"^//room\sdisconnect$")] = DisconnectFromCurrenctRoom,
                [new Regex(@"^//room\sdisconnect\s\w+$")] = DisconnectFromRoom,
                [new Regex(@"^//room\sdisconnect\s\w+\s-l\s\w+$")] = KickUserOutRoom,
                [new Regex(@"^//room\sdisconnect\s\w+\s-l\s\w+\s-m\s\d*$")] = TemporaryKickUserOutRoom,
                [new Regex(@"^//room\smute\s-l\s\w+$")] = MuteUser,
                [new Regex(@"^//room\smute\s-l\s\w+\s-m\s\d*$")] = TemporaryMuteUser,
                [new Regex(@"^//room\sspeak\s-l\s\w+$")] = UnmuteUser,
            };

            Database = uow;
            _userManager = userManager;
            _userValidator = userValidator;
            _roomValidator = roomValidator;
            _roomHandler = roomHandler;
        }

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommandAsync(User currentUser, Room currentRoom, string command, IHubCallerClients calledClients)
        {
            foreach (Regex keyCommand in roomCommands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    return roomCommands[keyCommand](currentUser, currentRoom, command, calledClients);
                }
            }
            //        return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
            return calledClients.Caller.SendAsync("ReceiveCommand", "Неверная команда");
        }

        /// <summary>
        /// Создать комнату.
        /// Метод создает запись с информацией об обычной комнате
        /// </summary>
        public Task CreateRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)$").Groups[1].Value;
            if (_roomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            string roomInfo = _roomHandler.CreateRoom(nameProcessedRoom, REGULAR_TYPE_ROOM, currentUser.Id);

            //        return clients.Caller.SendAsync("CreateRoom",
            //            CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} успешно создана."), roomInfo);
            return clients.Caller.SendAsync("ReceiveCommand", $"Комната {nameProcessedRoom} успешно создана.");
        }

        /// <summary>
        /// Создать приватную комнату.
        /// Метод создает запись с информацией о приватной комнате
        /// </summary>
        public Task CreatePrivateRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)\s-c$").Groups[1].Value;
            if (_roomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            string roomInfo = _roomHandler.CreateRoom(nameProcessedRoom, PRITVATE_TYPE_ROOM, currentUser.Id);

            //        return clients.Caller.SendAsync("CreateRoom",
            //            CommandHandler.CreateCommandInfo($"Приватная комната {nameProcessedRoom} успешно создана."), roomInfo);
            return clients.Caller.SendAsync("ReceiveCommand", $"Приватная комната {nameProcessedRoom} успешно создана.");
        }

        /// <summary>
        /// Создать чат-бот комнату.
        /// Метод создает запись с информацией о чат-бот комнате.
        /// </summary>
        public Task CreateBotRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)\s-b$").Groups[1].Value;
            if (_roomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }
            string roomInfo = _roomHandler.CreateRoom(nameProcessedRoom, BOT_TYPE_ROOM, currentUser.Id);

            //        return clients.Caller.SendAsync("CreateRoom",
            //            CommandHandler.CreateCommandInfo($"Чат-бот комната {nameProcessedRoom} успешно создана."), roomInfo);
            return clients.Caller.SendAsync("ReceiveCommand", $"Чат-бот комната {nameProcessedRoom} успешно создана.");
        }

        /// <summary>
        /// Удалить комнату.
        /// Метод удаляет запись с информацией об определенной группе.
        /// </summary>
        public async Task<Task> RemoveRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sremove\s(\w+)$").Groups[1].Value;

            if (Database.Rooms.FindByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            if (_roomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Комнаты с именем {nameProcessedRoom} не существует"));
            }

            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator" }, nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            string idProcessedRoom = Database.Rooms.FindByName(nameProcessedRoom).Id;

            List<string> members = Database.Members.GetMembersIds(idProcessedRoom).ToList();

            await clients.Users(members).SendAsync("RemoveRoomUsers",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} была удалена."), idProcessedRoom);

            Database.Rooms.DeleteById(idProcessedRoom);

            //        return clients.Caller.SendAsync("RemoveRoomCaller",
            //            CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} успешно удалена."), idProcessedRoom);
            return clients.Caller.SendAsync("ReceiveCommand", $"Комната {nameProcessedRoom} успешно удалена.");
        }

        /// <summary>
        /// Переименование комнаты.
        /// Метод изменяет имя заданной комнаты.
        /// </summary> 
        public async Task<Task> RenameRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            if (currentRoom.Id == MAIN_ROOM_ID)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string nameProcessedRoom = Regex.Match(command, @"//room\srename\s(\w+)$").Groups[1].Value;
            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator" }, currentRoom.RoomName))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (_roomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                currentRoom.RoomName = nameProcessedRoom;
                Database.Rooms.Update(currentRoom);

                List<string> members = Database.Members.GetMembersIds(currentRoom.Id).ToList();

                await clients.Users(members).SendAsync("RenameRoomUser", currentRoom.Id, nameProcessedRoom);

                //            return clients.Caller.SendAsync("RenameRoom",
                //                CommandHandler.CreateCommandInfo($"Имя комнаты было успешно изменено на {nameProcessedRoom}"),
                //                currentRoom.Id, nameProcessedRoom);
            }

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            return clients.Caller.SendAsync("ReceiveCommand", $"Комната c именем {nameProcessedRoom} уже занята.");
        }


        /// <summary>
        /// Подключение к комнате.
        /// Метод добавляет запись с информацией о членстве пользователя в определенной группе.
        /// </summary>
        public async Task<Task> ConnectionToRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[1].Value;

            if (Database.Rooms.FindByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[2].Value;

            string idProcessedRoom = Database.Rooms.FindByName(nameProcessedRoom)?.Id;
            string idProcessedUser = (await _userManager.FindByNameAsync(userNameProcessedUser))?.Id;


            if (idProcessedRoom == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} не найдена."));
            }

            if (idProcessedUser == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не найден."));
            }

            if (Database.Rooms.FindByName(nameProcessedRoom).TypeRoom.Id == BOT_TYPE_ROOM)
            {
                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }
            else if (Database.Rooms.FindByName(nameProcessedRoom).TypeRoom.Id == PRITVATE_TYPE_ROOM)
            {
                if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { }, nameProcessedRoom))
                {
                    //return clients.Caller.SendAsync("ReceiveCommand",
                    //    CommandHandler.CreateCommandInfo("Отказано в доступе."));
                }
            }

            if (_userValidator.IsUserInGroupById(idProcessedUser, idProcessedRoom))
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь уже состоит в этой группе"));
            }

            Database.Members.AddMember(idProcessedUser, idProcessedRoom);

            await clients.User(idProcessedUser.ToString()).SendAsync("ConnectRoom",
                JsonSerializer.Serialize<object>(new { roomId = idProcessedRoom, roomName = nameProcessedRoom }));

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был добавлен в комнату {nameProcessedRoom}"));
            return clients.Caller.SendAsync("ReceiveCommand", "Пользователь {loginProcessedUser} был добавлен в комнату {nameProcessedRoom}");
        }


        /// <summary>
        /// Отключиться от текущей комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в текущей группе.
        /// </summary>
        public Task DisconnectFromCurrenctRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            if (currentRoom.Id == MAIN_ROOM_ID)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }
            Database.Members.DeleteMember(currentUser.Id, currentRoom.Id);

            return clients.Caller.SendAsync("DisconnectFromCurrentRoom");
        }

        /// <summary>
        /// Отключиться от комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в определенной комнате.
        /// </summary>
        public Task DisconnectFromRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)$").Groups[1].Value;

            if (Database.Rooms.FindByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string idProcessedRoom = Database.Rooms.FindByName(nameProcessedRoom)?.Id;

            if (idProcessedRoom == null)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} не найдена."));
            }

            if (_userValidator.IsUserInGroupById(currentUser.Id, idProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Вы не состоите в комнате {nameProcessedRoom}."));
            }

            Database.Members.DeleteMember(currentUser.Id, idProcessedRoom);

            //        return clients.Caller.SendAsync("DisconnectFromRoom",
            //            CommandHandler.CreateCommandInfo($"Вы покинули комнату {nameProcessedRoom}"), idProcessedRoom);
            return clients.Caller.SendAsync("ReceiveCommand", $"Вы покинули комнату {nameProcessedRoom}");
        }

        /// <summary>
        /// Исключить пользователя из комнаты.
        /// Метод добавляет запись с информацией о изгнании пользователя из комнаты.
        /// </summary>
        public async Task<Task> KickUserOutRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)$").Groups[1].Value;
            if (Database.Rooms.FindByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)$").Groups[2].Value;

            string idProcessedRoom = Database.Rooms.FindByName(nameProcessedRoom)?.Id;
            string idProcessedUser = (await _userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            string resultChecking = CheckDataForKickedUser(nameProcessedRoom, userNameProcessedUser, idProcessedRoom, idProcessedUser);

            if (string.Empty != resultChecking)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(resultChecking));
            }

            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator", "Moderator" }, nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnkick = TimeComputer.CalculateUnlockDate(command + " -m 60", @"//room\sdisconnect\s\w+\s-l\s\w+\s-m\s(60)$");
            Database.KickedOuts.AddKickedUser(idProcessedUser, idProcessedRoom, dateUnkick);

            await clients.User(idProcessedUser.ToString()).SendAsync("DisconnectFromRoomUser", idProcessedRoom);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был исключен из комнаты {nameProcessedRoom} на " +
            //                $"{Math.Round((dateUnkick - DateTime.Now).TotalMinutes)} минут."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} был исключен из комнаты");
        }

        /// <summary>
        /// Временное исключение пользователя.
        /// Метод добавляет запись с информацией о временном изгнании пользователя из комнаты.
        /// </summary>
        public async Task<Task> TemporaryKickUserOutRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[1].Value;
            if (Database.Rooms.FindByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[2].Value;
     
            string idProcessedRoom = Database.Rooms.FindByName(nameProcessedRoom)?.Id;

            string idProcessedUser = (await _userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            string resultChecking = CheckDataForKickedUser(nameProcessedRoom, userNameProcessedUser, idProcessedRoom, idProcessedUser);

            if (string.Empty != resultChecking)
            {
                return clients.Caller.SendAsync("ReceiveCommand", resultChecking);
            }

            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator", "Moderator" }, nameProcessedRoom))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnkick = TimeComputer.CalculateUnlockDate(command, @"//room\sdisconnect\s\w+\s-l\s\w+\s-m\s(\d*)$");
            Database.KickedOuts.AddKickedUser(idProcessedUser, idProcessedRoom, dateUnkick);

            await clients.User(idProcessedUser.ToString()).SendAsync("DisconnectFromRoomUser", idProcessedRoom);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был исключен из комнаты {nameProcessedRoom} на " +
            //                $"{Math.Round((dateUnkick - DateTime.Now).TotalMinutes)} минут."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} был исключен из комнаты");
        }

        /// <summary>
        /// Метод добавляет запись с информацией о муте пользователя.  
        /// </summary>
        public async Task<Task> MuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)$").Groups[1].Value;

            string idProcessedUser = (await _userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} не найден.");
            }

            if (_userValidator.IsUserInGroupById(idProcessedUser, currentRoom.Id))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не состоит комнате."));
            }

            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command + " -m 10", @"//room\smute\s-l\s\w+\s-m\s(10)$");
            Database.MutedUsers.AddMutedUser(idProcessedUser, currentRoom.Id, dateUnmute);

            await clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", currentRoom.Id);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был приглушен на " +
            //                $"{Math.Round((dateUnmute - DateTime.Now).TotalMinutes)} минут."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} был приглушен");
        }

        /// <summary>
        /// Временное приглушение пользователя.
        /// Метод добавляет запись с информацией о временном муте пользователя. 
        /// </summary>
        public async Task<Task> TemporaryMuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)\s-m\s\d*$").Groups[1].Value;
            string idProcessedUser = (await _userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} не найден.");
            }

            if (_userValidator.IsUserInGroupById(idProcessedUser, currentRoom.Id))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не состоит комнате."));
            }

            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command, @"//room\smute\s-l\s\w+\s-m\s(\d*)$");
            Database.MutedUsers.AddMutedUser(idProcessedUser, currentRoom.Id, dateUnmute);

            await clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", currentRoom.Id);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был приглушен на " +
            //                $"{Math.Round((dateUnmute - DateTime.Now).TotalMinutes)} минут."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} был приглушен");
        }

        /// <summary>
        /// Метод удаляет запись из таблицы о пользователе который был приглушен
        /// </summary>
        public async Task<Task> UnmuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\sspeak\s-l\s(\w+)$").Groups[1].Value;
            string idProcessedUser = (await _userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo("Пользователь {loginProcessedUser} не найден."));
            }

            if (await _roomValidator.CommandAccessCheckAsync(currentUser, new string[] { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (_userValidator.IsUserInGroupById(idProcessedUser, currentRoom.Id))
            {
                //            return clients.Caller.SendAsync("ReceiveCommand",
                //                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не состоит группе."));
            }

            Database.MutedUsers.DeleteMutedUser(idProcessedUser, currentRoom.Id);
            await clients.User(idProcessedUser.ToString()).SendAsync("UnmutedUser",
                CommandHandler.CreateCommandInfo($"У вас снова есть возможность писать сообщения."), currentRoom.Id);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} размучен."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userNameProcessedUser} размучен.");
        }

        /// <summary>
        /// Метод проверяет существует ли пользователь к которому применяет команда, 
        /// существует ли комната из которой выгоняют.
        /// Состоит ли пользователь в данной группе.
        /// </summary>
        public string CheckDataForKickedUser(string nameProcessedRoom, string userNameProcessedUser, string idProcessedRoom, string idProcessedUser)
        {
            if (idProcessedRoom == null)
            {
                return $"Комната {nameProcessedRoom} не найдена.";
            }

            if (idProcessedUser == null)
            {
                return $"Пользователь {userNameProcessedUser} не найден.";
            }

            if (_userValidator.IsUserInGroupById(idProcessedUser, idProcessedRoom))
            {
                return $"Пользователь {userNameProcessedUser} не состоит комнате.";
            }
            return string.Empty;
        }
    }
}