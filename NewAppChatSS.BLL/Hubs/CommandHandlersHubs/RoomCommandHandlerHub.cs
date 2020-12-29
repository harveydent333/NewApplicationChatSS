using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Constants;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.BLL.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с комнатой
    /// </summary>
    public class RoomCommandHandlerHub : Hub, IRoomCommandHandler
    {
        private readonly Dictionary<Regex, Func<User, Room, string, IHubCallerClients, Task>> roomCommands;

        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;
        private readonly IRoomValidator roomValidator;
        private readonly IRoomHandler roomHandler;

        private readonly IMutedUserRepository mutedUserRepository;
        private readonly IKickedOutRepository kickedOutRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IMemberRepository memberRepository;

        public RoomCommandHandlerHub(
            IMemberRepository memberRepository,
            IMutedUserRepository mutedUserRepository,
            IKickedOutRepository kickedOutRepository,
            IRoomRepository roomRepository,
            UserManager<User> userManager,
            IUserValidator userValidator,
            IRoomValidator roomValidator,
            IRoomHandler roomHandler)
        {
            roomCommands = new Dictionary<Regex, Func<User, Room, string, IHubCallerClients, Task>>
            {
                [new Regex(@"^//room\screate\s([0-9A-z])+\s-b$")] = CreateBotRoom,
                [new Regex(@"^//room\screate\s([0-9A-z])+\s-c$")] = CreatePrivateRoom,
                [new Regex(@"^//room\screate\s([0-9A-z])+$")] = CreateRoom,
                [new Regex(@"^//room\sremove\s([0-9A-z])+$")] = RemoveRoom,
                [new Regex(@"^//room\srename\s([0-9A-z])+$")] = RenameRoom,
                [new Regex(@"^//room\sconnect\s([0-9A-z])+\s-l\s.+$")] = ConnectionToRoom,
                [new Regex(@"^//room\sdisconnect$")] = DisconnectFromCurrenctRoom,
                [new Regex(@"^//room\sdisconnect\s([0-9A-z])+$")] = DisconnectFromRoom,
                [new Regex(@"^//room\sdisconnect\s([0-9A-z])+\s-l\s.+$")] = KickUserOutRoom,
                [new Regex(@"^//room\sdisconnect\s([0-9A-z])+\s-l\s.+\s-m\s\d*$")] = TemporaryKickUserOutRoom,
                [new Regex(@"^//room\smute\s-l\s([0-9A-z])+$")] = MuteUser,
                [new Regex(@"^//room\smute\s-l\s([0-9A-z])+\s-m\s\d*$")] = TemporaryMuteUser,
                [new Regex(@"^//room\sspeak\s-l\s([0-9A-z])+$")] = UnmuteUser,
            };

            this.roomRepository = roomRepository;
            this.kickedOutRepository = kickedOutRepository;
            this.mutedUserRepository = mutedUserRepository;
            this.memberRepository = memberRepository;

            this.userManager = userManager;
            this.userValidator = userValidator;
            this.roomValidator = roomValidator;
            this.roomHandler = roomHandler;
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

            return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
        }

        /// <summary>
        /// Создать комнату.
        /// Метод создает запись с информацией об обычной комнате
        /// </summary>
        private async Task<Task> CreateRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)$").Groups[1].Value;

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            string roomInfo = await roomHandler.CreateRoom(nameProcessedRoom, GlobalConstants.RegularRoomType, currentUser.Id);

            return clients.Caller.SendAsync(
                "CreateRoom",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} успешно создана."),
                roomInfo);
        }

        /// <summary>
        /// Создать приватную комнату.
        /// Метод создает запись с информацией о приватной комнате
        /// </summary>
        private async Task<Task> CreatePrivateRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)\s-c$").Groups[1].Value;

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            string roomInfo = await roomHandler.CreateRoom(nameProcessedRoom, GlobalConstants.PrivateRoomType, currentUser.Id);

            return clients.Caller.SendAsync(
                "CreateRoom",
                CommandHandler.CreateCommandInfo($"Приватная комната {nameProcessedRoom} успешно создана."),
                roomInfo);
        }

        /// <summary>
        /// Создать чат-бот комнату.
        /// Метод создает запись с информацией о чат-бот комнате.
        /// </summary>
        private async Task<Task> CreateBotRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(.+)\s-b$").Groups[1].Value;

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            string roomInfo = await roomHandler.CreateRoom(nameProcessedRoom, GlobalConstants.BotRoomType, currentUser.Id);

            return clients.Caller.SendAsync(
                "CreateRoom",
                CommandHandler.CreateCommandInfo($"Чат-бот комната {nameProcessedRoom} успешно создана."),
                roomInfo);
        }

        /// <summary>
        /// Удалить комнату.
        /// Метод удаляет запись с информацией об определенной группе.
        /// </summary>
        private async Task<Task> RemoveRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sremove\s(\w+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom?.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            if (!await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комнаты с именем {nameProcessedRoom} не существует"));
            }

            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator" }, nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            var members = await memberRepository.GetAsync(new MemberModel { RoomId = processedRoom.Id });
            var memberIds = members.Select(m => m.UserId).ToList();

            await clients.Users(memberIds).SendAsync(
                "RemoveRoomUsers",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} была удалена."),
                processedRoom.Id);

            var deleteRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { processedRoom.Id } });
            await roomRepository.DeleteAsync(deleteRoom);

            return clients.Caller.SendAsync(
                "RemoveRoomCaller",
                CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} успешно удалена."),
                processedRoom.Id);
        }

        /// <summary>
        /// Переименование комнаты.
        /// Метод изменяет имя заданной комнаты.
        /// </summary>
        private async Task<Task> RenameRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            if (currentRoom.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string nameProcessedRoom = Regex.Match(command, @"//room\srename\s(\w+)$").Groups[1].Value;
            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната с именем {nameProcessedRoom} уже занята"));
            }

            currentRoom.RoomName = nameProcessedRoom;

            await roomRepository.ModifyAsync(currentRoom);

            var members = await memberRepository.GetAsync(new MemberModel { RoomId = currentRoom.Id });
            var memberIds = members.Select(m => m.UserId).ToList();

            await clients.Users(memberIds).SendAsync("RenameRoomUser", currentRoom.Id, nameProcessedRoom);

            return clients.Caller.SendAsync(
                "RenameRoom",
                CommandHandler.CreateCommandInfo($"Имя комнаты было успешно изменено на {nameProcessedRoom}"),
                currentRoom.Id,
                nameProcessedRoom);
        }

        /// <summary>
        /// Подключение к комнате.
        /// Метод добавляет запись с информацией о членстве пользователя в определенной группе.
        /// </summary>
        private async Task<Task> ConnectionToRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom, IncludeTypeRoom = true });

            if (processedRoom?.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[2].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (processedRoom == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} не найдена."));
            }

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не найден."));
            }

            if (processedRoom.TypeRoom.Id == GlobalConstants.BotRoomType)
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }
            else if (processedRoom.TypeRoom.Id == GlobalConstants.PrivateRoomType)
            {
                if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { }, nameProcessedRoom))
                {
                    return clients.Caller.SendAsync(
                        "ReceiveCommand",
                        CommandHandler.CreateCommandInfo("Отказано в доступе."));
                }
            }

            if (await userValidator.IsUserInGroupById(idProcessedUser, processedRoom.Id))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь уже состоит в этой группе"));
            }

            var member = new Member
            {
                UserId = idProcessedUser,
                RoomId = processedRoom.Id
            };

            await memberRepository.AddAsync(member);

            await clients.User(idProcessedUser.ToString()).SendAsync(
                "ConnectRoom",
                JsonSerializer.Serialize<object>(new { roomId = processedRoom.Id, roomName = nameProcessedRoom }));

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} был добавлен в комнату {nameProcessedRoom}"));
        }

        /// <summary>
        /// Отключиться от текущей комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в текущей группе.
        /// </summary>
        private async Task<Task> DisconnectFromCurrenctRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            if (currentRoom.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            var deleteMember = await memberRepository.GetFirstOrDefaultAsync(new MemberModel { UserId = currentUser.Id, RoomId = currentRoom.Id });

            await memberRepository.DeleteAsync(deleteMember);

            return clients.Caller.SendAsync("DisconnectFromCurrentRoom");
        }

        /// <summary>
        /// Отключиться от комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в определенной комнате.
        /// </summary>
        private async Task<Task> DisconnectFromRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom?.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            if (processedRoom == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Комната {nameProcessedRoom} не найдена."));
            }

            if (await userValidator.IsUserInGroupById(currentUser.Id, processedRoom.Id))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Вы не состоите в комнате {nameProcessedRoom}."));
            }

            var deleteMember = await memberRepository.GetFirstOrDefaultAsync(new MemberModel { UserId = currentUser.Id, RoomId = currentRoom.Id });
            // TODO: проверка deleteMember
            await memberRepository.DeleteAsync(deleteMember);

            return clients.Caller.SendAsync(
                "DisconnectFromRoom",
                CommandHandler.CreateCommandInfo($"Вы покинули комнату {nameProcessedRoom}"),
                processedRoom.Id);
        }

        /// <summary>
        /// Исключить пользователя из комнаты.
        /// Метод добавляет запись с информацией о изгнании пользователя из комнаты.
        /// </summary>
        private async Task<Task> KickUserOutRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(.+)\s-l\s(.+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom?.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(.+)\s-l\s(.+)$").Groups[2].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            string resultChecking = await CheckDataForKickedUser(nameProcessedRoom, userNameProcessedUser, processedRoom.Id, idProcessedUser);

            if (resultChecking != string.Empty)
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(resultChecking));
            }

            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator", "Moderator" }, nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            var kickedIdRooms = (await kickedOutRepository.GetAsync(new KickedOutModel { UserId = idProcessedUser })).Select(k => k.RoomId);

            if (kickedIdRooms.Contains(processedRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Пользователь уже был исключен из этой команты"));
            }

            DateTime dateUnkick = TimeComputer.CalculateUnlockDate(command + " -m 60", @"//room\sdisconnect\s\w+\s-l\s\w+\s-m\s(60)$");

            var kikedOut = new KickedOut
            {
                UserId = idProcessedUser,
                RoomId = processedRoom.Id,
                DateUnkick = dateUnkick,
            };

            await kickedOutRepository.AddAsync(kikedOut);

            await clients.User(idProcessedUser.ToString()).SendAsync("DisconnectFromRoomUser", processedRoom.Id);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} был исключен из комнаты {nameProcessedRoom} на " +
                    $"{Math.Round((dateUnkick - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Временное исключение пользователя.
        /// Метод добавляет запись с информацией о временном изгнании пользователя из комнаты.
        /// </summary>
        private async Task<Task> TemporaryKickUserOutRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom?.Id == GlobalConstants.MainRoomId)
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo($"Невозможно применить к главной комнате."));
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[2].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            string resultChecking = await CheckDataForKickedUser(nameProcessedRoom, userNameProcessedUser, processedRoom.Id, idProcessedUser);

            if (resultChecking != string.Empty)
            {
                return clients.Caller.SendAsync("ReceiveCommand", resultChecking);
            }

            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator", "Moderator" }, nameProcessedRoom))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            var kickedIdRooms = (await kickedOutRepository.GetAsync(new KickedOutModel { UserId = idProcessedUser })).Select(k => k.RoomId);

            if (kickedIdRooms.Contains(processedRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Пользователь уже был исключен из этой команты"));
            }

            DateTime dateUnkick = TimeComputer.CalculateUnlockDate(command, @"//room\sdisconnect\s\w+\s-l\s\w+\s-m\s(\d*)$");

            var kikedOut = new KickedOut
            {
                UserId = idProcessedUser,
                RoomId = processedRoom.Id,
                DateUnkick = dateUnkick,
            };

            await kickedOutRepository.AddAsync(kikedOut);

            await clients.User(idProcessedUser.ToString()).SendAsync("DisconnectFromRoomUser", processedRoom.Id);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} был исключен из комнаты {nameProcessedRoom} на " +
                    $"{Math.Round((dateUnkick - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Метод добавляет запись с информацией о муте пользователя.
        /// </summary>
        private async Task<Task> MuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)$").Groups[1].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не найден."));
            }

            if (await userValidator.IsUserInGroupById(idProcessedUser, currentRoom.Id))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не состоит комнате."));
            }

            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            var mutedUsers = await mutedUserRepository.GetAsync(new MutedUserModel { UserId = idProcessedUser });
            var idMutedRooms = mutedUsers.Select(m => m.RoomId);

            if (idMutedRooms.Contains(currentRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Пользователь уже был приглушен в этой команате"));
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command + " -m 10", @"//room\smute\s-l\s\w+\s-m\s(10)$");

            var mutedUser = new MutedUser
            {
                UserId = idProcessedUser,
                RoomId = currentRoom.Id,
                DateUnmute = dateUnmute
            };

            await mutedUserRepository.AddAsync(mutedUser);

            await clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", currentRoom.Id);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} был приглушен на " +
                    $"{Math.Round((dateUnmute - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Временное приглушение пользователя.
        /// Метод добавляет запись с информацией о временном муте пользователя.
        /// </summary>
        private async Task<Task> TemporaryMuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)\s-m\s\d*$").Groups[1].Value;
            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не найден."));
            }

            if (await userValidator.IsUserInGroupById(idProcessedUser, currentRoom.Id))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не состоит комнате."));
            }

            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            var mutedUsers = await mutedUserRepository.GetAsync(new MutedUserModel { UserId = idProcessedUser });
            var idMutedRooms = mutedUsers.Select(m => m.RoomId);

            if (idMutedRooms.Contains(currentRoom.Id))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Пользователь уже был приглушен в этой команате"));
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command, @"//room\smute\s-l\s\w+\s-m\s(\d*)$");

            var mutedUser = new MutedUser
            {
                UserId = idProcessedUser,
                RoomId = currentRoom.Id,
                DateUnmute = dateUnmute
            };

            await mutedUserRepository.AddAsync(mutedUser);

            await clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", currentRoom.Id);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} был приглушен на " +
                    $"{Math.Round((dateUnmute - DateTime.Now).TotalMinutes)} минут."));
        }

        /// <summary>
        /// Метод удаляет запись из таблицы о пользователе который был приглушен
        /// </summary>
        private async Task<Task> UnmuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\sspeak\s-l\s(\w+)$").Groups[1].Value;
            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo("Пользователь {userNameProcessedUser} не найден."));
            }

            if (!await roomValidator.CommandAccessCheckAsync(currentUser, new List<string> { "Administrator", "Moderator" }, currentRoom.RoomName))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (await userValidator.IsUserInGroupById(idProcessedUser, currentRoom.Id))
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} не состоит группе."));
            }

            var deleteMutedUser = await mutedUserRepository.GetFirstOrDefaultAsync(new MutedUserModel { UserId = idProcessedUser, RoomId = currentRoom.Id });
            // TODO: if deleteMutedUser == null
            await mutedUserRepository.DeleteAsync(deleteMutedUser);

            await clients.User(idProcessedUser.ToString()).SendAsync(
                "UnmutedUser",
                CommandHandler.CreateCommandInfo($"У вас снова есть возможность писать сообщения."),
                currentRoom.Id);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {userNameProcessedUser} размучен."));
        }

        /// <summary>
        /// Метод проверяет существует ли пользователь к которому применяет команда,
        /// существует ли комната из которой выгоняют.
        /// Состоит ли пользователь в данной группе.
        /// </summary>
        private async Task<string> CheckDataForKickedUser(string nameProcessedRoom, string userNameProcessedUser, string idProcessedRoom, string idProcessedUser)
        {
            if (idProcessedRoom == null)
            {
                return $"Комната {nameProcessedRoom} не найдена.";
            }

            if (idProcessedUser == null)
            {
                return $"Пользователь {userNameProcessedUser} не найден.";
            }

            if (await userValidator.IsUserInGroupById(idProcessedUser, idProcessedRoom))
            {
                return $"Пользователь {userNameProcessedUser} не состоит комнате.";
            }

            return string.Empty;
        }
    }
}