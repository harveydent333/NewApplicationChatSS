using Infrastructure.ModelHandlers;
using Data.Models.KickedOuts;
using Data.Models.Members;
using Data.Models.MutedUsers;
using Data.Models.Rooms;
using Data.Models.Users;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastucture.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с комнатой
    /// </summary>
    public class RoomCommandHandlerHub : Hub
    {
        const Int32 REGULAR_TYPE_ROOM = 1;
        const Int32 PRITVATE_TYPE_ROOM = 2;
        const Int32 BOT_TYPE_ROOM = 3;

        const String MAIN_ROOM_ID = "1";

        private static IUserRepository userRepository;
        private static IRoomRepository roomRepository;
        private static IMemberRepository memberRepository;
        private static IKickedOutsRepository kickedOutsRepository;
        private static IMutedUserRepository mutedUserRepository;

        public RoomCommandHandlerHub(IUserRepository userRep, IRoomRepository roomRep, IMemberRepository memberRep, IKickedOutsRepository kickedOutsRep, IMutedUserRepository mutedUserRep)
        {
            userRepository = userRep;
            roomRepository = roomRep;
            memberRepository = memberRep;
            kickedOutsRepository = kickedOutsRep;
            mutedUserRepository = mutedUserRep;
        }

        private static Dictionary<Regex, Func<User, Room, String, IHubCallerClients, Task>> roomCommands = new Dictionary<Regex, Func<User, Room, String, IHubCallerClients, Task>>
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

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommand(User currentUser, Room currentRoom, String command, IHubCallerClients calledClients)
        {
            foreach (Regex keyCommand in roomCommands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    return roomCommands[keyCommand](currentUser, currentRoom, command, calledClients);
                }
            }

            return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
        }

        /// <summary>
        /// Создать комнату.
        /// Метод создает запись с информацией об обычной комнате
        /// </summary>
        public static Task CreateRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)$").Groups[1].Value;
            if (!RoomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            String roomInfo = RoomHandler.CreateRoom(nameProcessedRoom, REGULAR_TYPE_ROOM, currentUser.Id);

            return clients.Caller.SendAsync("CreateRoom",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} успешно создана."), roomInfo);
        }

        /// <summary>
        /// Создать приватную комнату.
        /// Метод создает запись с информацией о приватной комнате
        /// </summary>
        public static Task CreatePrivateRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)\s-c$").Groups[1].Value;
            if (!RoomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            String roomInfo = RoomHandler.CreateRoom(nameProcessedRoom, PRITVATE_TYPE_ROOM, currentUser.Id);

            return clients.Caller.SendAsync("CreateRoom",
                CommandHandler.CreateCommandInfo($"Приватная комната {nameProcessedRoom} успешно создана."), roomInfo);
        }

        /// <summary>
        /// Создать чат-бот комнату.
        /// Метод создает запись с информацией о чат-бот комнате.
        /// </summary>
        public static Task CreateBotRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)\s-b$").Groups[1].Value;
            if (!RoomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }
            String roomInfo = RoomHandler.CreateRoom(nameProcessedRoom, BOT_TYPE_ROOM, currentUser.Id);

            return clients.Caller.SendAsync("CreateRoom",
                CommandHandler.CreateCommandInfo($"Чат-бот комната {nameProcessedRoom} успешно создана."), roomInfo);
        }

        /// <summary>
        /// Удалить комнату.
        /// Метод удаляет запись с информацией об определенной группе.
        /// </summary>
        public static Task RemoveRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\sremove\s(\w+)$").Groups[1].Value;

            if (roomRepository.FindRoomByName(nameProcessedRoom).Id == "1")
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            if (RoomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комнаты с именем {nameProcessedRoom} не существует"));
            }

            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator" }, nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            String idProcessedRoom = roomRepository.FindRoomByName(nameProcessedRoom).Id;
            List<String> members = memberRepository.GetMembers(idProcessedRoom);

            clients.Users(members).SendAsync("RemoveRoomUsers",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} была удалена."), idProcessedRoom);

            roomRepository.DeleteRoom(idProcessedRoom);

            return clients.Caller.SendAsync("RemoveRoomCaller",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} успешно удалена."), idProcessedRoom);
        }

        /// <summary>
        /// Переименование комнаты.
        /// Метод изменяет имя заданной комнаты.
        /// </summary> 
        public static Task RenameRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            if (currentRoom.Id == MAIN_ROOM_ID)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            String nameProcessedRoom = Regex.Match(command, @"//room\srename\s(\w+)$").Groups[1].Value;
            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (RoomValidator.UniquenessCheckRoom(nameProcessedRoom))
            {
                currentRoom.RoomName = nameProcessedRoom;
                roomRepository.EditRoom(currentRoom);
                List<String> members = memberRepository.GetMembers(currentRoom.Id);

                clients.Users(members).SendAsync("RenameRoomUser", currentRoom.Id, nameProcessedRoom);

                return clients.Caller.SendAsync("RenameRoom",
                    CommandHandler.CreateCommandInfo($"Имя комнаты было успешно изменено на {nameProcessedRoom}"),
                    currentRoom.Id, nameProcessedRoom);
            }

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
        }


        /// <summary>
        /// Подключение к комнате.
        /// Метод добавляет запись с информацией о членстве пользователя в определенной группе.
        /// </summary>
        public static Task ConnectionToRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[1].Value;

            if (roomRepository.FindRoomByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            String loginProcessedUser = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[2].Value;

            String idProcessedRoom = roomRepository.FindRoomByName(nameProcessedRoom)?.Id;
            Int32? idProcessedUser = userRepository.FindUserByLogin(loginProcessedUser)?.Id;

            if (idProcessedRoom == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} не найдена."));
            }

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не найден."));
            }

            if (roomRepository.FindRoomByName(nameProcessedRoom).TypeRoom.Id == BOT_TYPE_ROOM)
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }
            else if (roomRepository.FindRoomByName(nameProcessedRoom).TypeRoom.Id == PRITVATE_TYPE_ROOM)
            {
                if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { }, nameProcessedRoom))
                {
                    return clients.Caller.SendAsync("ReceiveCommand",
                        CommandHandler.CreateCommandInfo("Отказано в доступе."));
                }
            }

            if (UserValidator.IsUserInGroup(idProcessedUser, idProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь уже состоит в этой группе"));
            }

            memberRepository.AddMember(idProcessedUser, idProcessedRoom);

            clients.User(idProcessedUser.ToString()).SendAsync("ConnectRoom",
                JsonSerializer.Serialize<object>(new { roomId = idProcessedRoom, roomName = nameProcessedRoom }));

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был добавлен в комнату {nameProcessedRoom}"));
        }


        /// <summary>
        /// Отключиться от текущей комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в текущей группе.
        /// </summary>
        public static Task DisconnectFromCurrenctRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            if (currentRoom.Id == MAIN_ROOM_ID)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            memberRepository.DeleteMember(currentUser.Id, currentRoom.Id);

            return clients.Caller.SendAsync("DisconnectFromCurrentRoom");
        }

        /// <summary>
        /// Отключиться от комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в определенной комнате.
        /// </summary>
        public static Task DisconnectFromRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)$").Groups[1].Value;

            if (roomRepository.FindRoomByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            String idProcessedRoom = roomRepository.FindRoomByName(nameProcessedRoom)?.Id;
            if (idProcessedRoom == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} не найдена."));
            }

            if (!UserValidator.IsUserInGroup(currentUser.Id, idProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Вы не состоите в комнате {nameProcessedRoom}."));
            }

            memberRepository.DeleteMember(currentUser.Id, idProcessedRoom);

            return clients.Caller.SendAsync("DisconnectFromRoom",
                CommandHandler.CreateCommandInfo($"Вы покинули комнату {nameProcessedRoom}"), idProcessedRoom);
        }

        /// <summary>
        /// Исключить пользователя из комнаты.
        /// Метод добавляет запись с информацией о изгнании пользователя из комнаты.
        /// </summary>
        public static Task KickUserOutRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)$").Groups[1].Value;
            if (roomRepository.FindRoomByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            String loginProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)$").Groups[2].Value;

            String idProcessedRoom = roomRepository.FindRoomByName(nameProcessedRoom)?.Id;
            Int32? idProcessedUser = userRepository.FindUserByLogin(loginProcessedUser)?.Id;

            String resultChecking = CheckDataForKickedUser(nameProcessedRoom, loginProcessedUser, idProcessedRoom, idProcessedUser);

            if (String.Empty != resultChecking)
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(resultChecking));
            }

            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator", "Moderator" }, nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnkick = TimeComputer.CalculateUnlockDate(command + " -m 60", @"//room\sdisconnect\s\w+\s-l\s\w+\s-m\s(60)$");
            kickedOutsRepository.AddKickeddUser(idProcessedUser, idProcessedRoom, dateUnkick);

            clients.User(idProcessedUser.ToString()).SendAsync("DisconnectFromRoomUser", idProcessedRoom);

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был исключен из комнаты {nameProcessedRoom} на " +
                    $"{Math.Round((dateUnkick - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Временное исключение пользователя.
        /// Метод добавляет запись с информацией о временном изгнании пользователя из комнаты.
        /// </summary>
        public static Task TemporaryKickUserOutRoom(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[1].Value;
            if (roomRepository.FindRoomByName(nameProcessedRoom).Id == MAIN_ROOM_ID)
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            String loginProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[2].Value;
            String idProcessedRoom = roomRepository.FindRoomByName(nameProcessedRoom)?.Id;
            Int32? idProcessedUser = userRepository.FindUserByLogin(loginProcessedUser)?.Id;

            String resultChecking = CheckDataForKickedUser(nameProcessedRoom, loginProcessedUser, idProcessedRoom, idProcessedUser);

            if (String.Empty != resultChecking)
            {
                return clients.Caller.SendAsync("ReceiveCommand", resultChecking);
            }

            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator", "Moderator" }, nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnkick = TimeComputer.CalculateUnlockDate(command, @"//room\sdisconnect\s\w+\s-l\s\w+\s-m\s(\d*)$");
            kickedOutsRepository.AddKickeddUser(idProcessedUser, idProcessedRoom, dateUnkick);

            clients.User(idProcessedUser.ToString()).SendAsync("DisconnectFromRoomUser", idProcessedRoom);

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был исключен из комнаты {nameProcessedRoom} на " +
                    $"{Math.Round((dateUnkick - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Метод добавляет запись с информацией о муте пользователя.  
        /// </summary>
        public static Task MuteUser(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String loginProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)$").Groups[1].Value;
            Int32? idProcessedUser = userRepository.FindUserByLogin(loginProcessedUser)?.Id;

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не найден."));
            }

            if (!UserValidator.IsUserInGroup(idProcessedUser, currentRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не состоит комнате."));
            }

            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command + " -m 10", @"//room\smute\s-l\s\w+\s-m\s(10)$");
            mutedUserRepository.AddMutedUser(idProcessedUser, currentRoom.Id, dateUnmute);

            clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", currentRoom.Id);

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был приглушен на " +
                    $"{Math.Round((dateUnmute - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Временное приглушение пользователя.
        /// Метод добавляет запись с информацией о временном муте пользователя. 
        /// </summary>
        public static Task TemporaryMuteUser(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String loginProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)\s-m\s\d*$").Groups[1].Value;
            Int32? idProcessedUser = userRepository.FindUserByLogin(loginProcessedUser)?.Id;

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не найден."));
            }

            if (!UserValidator.IsUserInGroup(idProcessedUser, currentRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не состоит комнате."));
            }

            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command, @"//room\smute\s-l\s\w+\s-m\s(\d*)$");//
            mutedUserRepository.AddMutedUser(idProcessedUser, currentRoom.Id, dateUnmute);

            clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", currentRoom.Id);

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} был приглушен на " +
                    $"{Math.Round((dateUnmute - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Метод удаляет запись из таблицы о пользователе который был приглушен
        /// </summary>
        public static Task UnmuteUser(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            String loginProcessedUser = Regex.Match(command, @"//room\sspeak\s-l\s(\w+)$").Groups[1].Value;
            Int32? idProcessedUser = userRepository.FindUserByLogin(loginProcessedUser)?.Id;

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo("Пользователь {loginProcessedUser} не найден."));
            }

            if (!RoomValidator.CommandAccessCheck(currentUser, new String[] { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (!UserValidator.IsUserInGroup(idProcessedUser, currentRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} не состоит группе."));
            }

            mutedUserRepository.DeleteMutedUser(idProcessedUser, currentRoom.Id);
            clients.User(idProcessedUser.ToString()).SendAsync("UnmutedUser",
                CommandHandler.CreateCommandInfo($"У вас снова есть возможность писать сообщения."), currentRoom.Id);

            return clients.Caller.SendAsync("ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {loginProcessedUser} размучен."));
        }

        /// <summary>
        /// Метод проверяет существует ли пользователь к которому применяет команда, 
        /// существует ли комната из которой выгоняют.
        /// Состоит ли пользователь в данной группе.
        /// </summary>
        public static String CheckDataForKickedUser(String nameProcessedRoom, String loginProcessedUser, String idProcessedRoom, Int32? idProcessedUser)
        {
            if (idProcessedRoom == null)
            {
                return $"Комната {nameProcessedRoom} не найдена.";
            }

            if (idProcessedUser == null)
            {
                return $"Пользователь {loginProcessedUser} не найден.";
            }

            if (!UserValidator.IsUserInGroup(idProcessedUser, idProcessedRoom))
            {
                return $"Пользователь {loginProcessedUser} не состоит комнате.";
            }

            // создать в KickedOut, , MutedUser - метод, который вернет запись, по переданному id_user и room_id и в ней уже 
            //  проверить, если дата разблокировки < даты сейчас. То выводим ПОЛЬЗОВАТЕЛЬ уже исключен
            return String.Empty;
        }
    }
}