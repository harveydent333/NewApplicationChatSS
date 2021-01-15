using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;
using NewAppChatSS.Hubs.Infrastructure;
using NewAppChatSS.Hubs.Interfaces.HubInterfaces;
using NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;

// TODO: Обработка исключение, в отдельный класс и возвращать нормально
namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с комнатой
    /// </summary>
    public class RoomCommandHandlerHub : AbstarctHub, IRoomCommandHandler
    {
        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;
        private readonly IRoomValidator roomValidator;
        private readonly IRoomHandler roomHandler;

        private readonly IMutedUserRepository mutedUserRepository;
        private readonly IKickedOutRepository kickedOutRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IMemberRepository memberRepository;

        public override Dictionary<Regex, Func<string, Task>> Commands { get; }

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
            Commands = new Dictionary<Regex, Func<string, Task>>
            {
                [new Regex(@"^//room\screate\s([0-9A-z])+\s-b$")] = CreateBotRoomAsync,
                [new Regex(@"^//room\screate\s([0-9A-z])+\s-c$")] = CreatePrivateRoomAsync,
                [new Regex(@"^//room\screate\s([0-9A-z])+$")] = CreateRoomAsync,
                [new Regex(@"^//room\sremove\s([0-9A-z])+$")] = RemoveRoomAsync,
                [new Regex(@"^//room\srename\s([0-9A-z])+$")] = RenameRoomAsync,
                [new Regex(@"^//room\sconnect\s([0-9A-z])+\s-l\s.+$")] = ConnectionToRoomAsync,
                [new Regex(@"^//room\sdisconnect$")] = DisconnectFromCurrenctRoomAsync,
                [new Regex(@"^//room\sdisconnect\s([0-9A-z])+$")] = DisconnectFromRoomAsync,
                [new Regex(@"^//room\sdisconnect\s([0-9A-z])+\s-l\s.+$")] = KickUserOutRoomAsync,
                [new Regex(@"^//room\sdisconnect\s([0-9A-z])+\s-l\s.+\s-m\s\d*$")] = TemporaryKickUserOutRoomAsync,
                [new Regex(@"^//room\smute\s-l\s([0-9A-z])+$")] = MuteUserAsync,
                [new Regex(@"^//room\smute\s-l\s([0-9A-z])+\s-m\s\d*$")] = TemporaryMuteUserAsync,
                [new Regex(@"^//room\sspeak\s-l\s([0-9A-z])+$")] = UnmuteUserAsync,
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
        /// Создать комнату.
        /// Метод создает запись с информацией об обычной комнате
        /// </summary>
        private async Task CreateRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)$").Groups[1].Value;

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNameAlreadyTaken));

                return;
            }

            string roomInfo = await roomHandler.CreateRoomAsync(nameProcessedRoom, GlobalConstants.RegularRoomType, user.Id);

            await clients.Caller.SendAsync(
                "CreateRoom",
                CommandHandler.CreateCommandInfo(InformationMessages.RoomHasBeenCreated),
                roomInfo);
        }

        /// <summary>
        /// Создать приватную комнату.
        /// Метод создает запись с информацией о приватной комнате
        /// </summary>
        private async Task CreatePrivateRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(\w+)\s-c$").Groups[1].Value;

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNameAlreadyTaken));

                return;
            }

            string roomInfo = await roomHandler.CreateRoomAsync(nameProcessedRoom, GlobalConstants.PrivateRoomType, user.Id);

            await clients.Caller.SendAsync(
                "CreateRoom",
                CommandHandler.CreateCommandInfo(InformationMessages.RoomHasBeenCreated),
                roomInfo);
        }

        /// <summary>
        /// Создать чат-бот комнату.
        /// Метод создает запись с информацией о чат-бот комнате.
        /// </summary>
        private async Task CreateBotRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\screate\s(.+)\s-b$").Groups[1].Value;

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNameAlreadyTaken));

                return;
            }

            string roomInfo = await roomHandler.CreateRoomAsync(nameProcessedRoom, GlobalConstants.BotRoomType, user.Id);

            await clients.Caller.SendAsync(
                "CreateRoom",
                CommandHandler.CreateCommandInfo(InformationMessages.RoomHasBeenCreated),
                roomInfo);
        }

        /// <summary>
        /// Удалить комнату.
        /// Метод удаляет запись с информацией об определенной группе.
        /// </summary>
        private async Task RemoveRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sremove\s(\w+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNotFound));
                return;
            }

            if (await CheckMainRoomAsync(processedRoom))
            {
                return;
            }

            if (!await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNotFound));
                return;
            }

            if (!await roomValidator.CommandAccessCheckAsync(user, new List<string> { RoleConstants.AdministratorRole }, nameProcessedRoom))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            var members = await memberRepository.GetAsync(new MemberModel { RoomId = processedRoom.Id });
            var memberIds = members.Select(m => m.UserId).ToList();

            await clients.Users(memberIds).SendAsync(
                "RemoveRoomUsers",
                CommandHandler.CreateCommandInfo(InformationMessages.RoomHasBeenRemoved),
                processedRoom.Id);

            var deleteRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { processedRoom.Id } });
            await roomRepository.DeleteAsync(deleteRoom);

            await clients.Caller.SendAsync(
                "RemoveRoomCaller",
                CommandHandler.CreateCommandInfo(InformationMessages.RoomHasBeenRemoved),
                processedRoom.Id);
        }

        /// <summary>
        /// Переименование комнаты.
        /// Метод изменяет имя заданной комнаты.
        /// </summary>
        private async Task RenameRoomAsync(string command)
        {
            if (await CheckMainRoomAsync(room))
            {
                return;
            }

            string nameProcessedRoom = Regex.Match(command, @"//room\srename\s(\w+)$").Groups[1].Value;

            if (!await roomValidator.CommandAccessCheckAsync(user, new List<string> { RoleConstants.AdministratorRole }, room.RoomName))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            if (await roomRepository.IsExistAsync(new RoomModel { RoomName = nameProcessedRoom }))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNameAlreadyTaken));
                return;
            }

            room.RoomName = nameProcessedRoom;

            await roomRepository.ModifyAsync(room);

            var members = await memberRepository.GetAsync(new MemberModel { RoomId = room.Id });
            var memberIds = members.Select(m => m.UserId).ToList();

            await clients.Users(memberIds).SendAsync("RenameRoomUser", room.Id, nameProcessedRoom);

            await clients.Caller.SendAsync(
                "RenameRoom", CommandHandler.CreateCommandInfo(InformationMessages.RoomNameHasBeenChanged), room.Id, nameProcessedRoom);
        }

        /// <summary>
        /// Подключение к комнате.
        /// Метод добавляет запись с информацией о членстве пользователя в определенной группе.
        /// </summary>
        private async Task ConnectionToRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom, IncludeTypeRoom = true });

            if (processedRoom == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNotFound));
                return;
            }

            if (await CheckMainRoomAsync(processedRoom))
            {
                return;
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sconnect\s(\w+)\s-l\s(\w+)$").Groups[2].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
                return;
            }

            if (processedRoom.TypeRoom.Id == GlobalConstants.BotRoomType)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }
            else if (processedRoom.TypeRoom.Id == GlobalConstants.PrivateRoomType)
            {
                if (!await roomValidator.CommandAccessCheckAsync(user, new List<string> { }, nameProcessedRoom))
                {
                    await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                    return;
                }
            }

            if (await userValidator.IsUserInGroupByIdAsync(idProcessedUser, processedRoom.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserIsAlreadyMemberOfRoom));
                return;
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

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenAdded));
        }

        /// <summary>
        /// Отключиться от текущей комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в текущей группе.
        /// </summary>
        private async Task DisconnectFromCurrenctRoomAsync(string command)
        {
            if (await CheckMainRoomAsync(room))
            {
                return;
            }

            var deleteMember = await memberRepository.GetFirstOrDefaultAsync(new MemberModel { UserId = user.Id, RoomId = room.Id });

            await memberRepository.DeleteAsync(deleteMember);

            await clients.Caller.SendAsync("DisconnectFromCurrentRoom");
        }

        /// <summary>
        /// Отключиться от комнаты.
        /// Метод удаляет запись с информацией о членстве пользователя в определенной комнате.
        /// </summary>
        private async Task DisconnectFromRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNotFound));
                return;
            }

            if (await CheckMainRoomAsync(processedRoom))
            {
                return;
            }

            if (await userValidator.IsUserInGroupByIdAsync(user.Id, processedRoom.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.YouIsNotMemberRoom));
                return;
            }

            var deleteMember = await memberRepository.GetFirstOrDefaultAsync(new MemberModel { UserId = user.Id, RoomId = room.Id });
            // TODO: проверка deleteMember
            await memberRepository.DeleteAsync(deleteMember);

            await clients.Caller.SendAsync(
                "DisconnectFromRoom",
                CommandHandler.CreateCommandInfo(InformationMessages.YouLeaveRoom),
                processedRoom.Id);
        }

        /// <summary>
        /// Исключить пользователя из комнаты.
        /// Метод добавляет запись с информацией о изгнании пользователя из комнаты.
        /// </summary>
        private async Task KickUserOutRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(.+)\s-l\s(.+)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNotFound));
                return;
            }

            if (await CheckMainRoomAsync(processedRoom))
            {
                return;
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(.+)\s-l\s(.+)$").Groups[2].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            string resultChecking = await CheckDataForKickedUserAsync(nameProcessedRoom, userNameProcessedUser, processedRoom.Id, idProcessedUser);

            if (resultChecking != string.Empty)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(resultChecking));
                return;
            }

            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await roomValidator.CommandAccessCheckAsync(user, acceptableRoles, nameProcessedRoom))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            var kickedIdRooms = (await kickedOutRepository.GetAsync(new KickedOutModel { UserId = idProcessedUser })).Select(k => k.RoomId);

            if (kickedIdRooms.Contains(processedRoom.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserStillKicked));
                return;
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

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenKicked));
        }

        /// <summary>
        /// Временное исключение пользователя.
        /// Метод добавляет запись с информацией о временном изгнании пользователя из комнаты.
        /// </summary>
        private async Task TemporaryKickUserOutRoomAsync(string command)
        {
            string nameProcessedRoom = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[1].Value;

            var processedRoom = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (processedRoom == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.RoomNotFound));
                return;
            }

            if (await CheckMainRoomAsync(processedRoom))
            {
                return;
            }

            string userNameProcessedUser = Regex.Match(command, @"//room\sdisconnect\s(\w+)\s-l\s(\w+)\s-m\s(\d*)$").Groups[2].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            string resultChecking = await CheckDataForKickedUserAsync(nameProcessedRoom, userNameProcessedUser, processedRoom.Id, idProcessedUser);

            if (resultChecking != string.Empty)
            {
                await clients.Caller.SendAsync("ReceiveCommand", resultChecking);
                return;
            }

            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await roomValidator.CommandAccessCheckAsync(user, acceptableRoles, nameProcessedRoom))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            var kickedIdRooms = (await kickedOutRepository.GetAsync(new KickedOutModel { UserId = idProcessedUser })).Select(k => k.RoomId);

            if (kickedIdRooms.Contains(processedRoom.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserStillKicked));
                return;
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

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenKicked));
        }

        /// <summary>
        /// Метод добавляет запись с информацией о муте пользователя.
        /// </summary>
        private async Task MuteUserAsync(string command)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)$").Groups[1].Value;

            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
                return;
            }

            if (await userValidator.IsUserInGroupByIdAsync(idProcessedUser, room.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserIsNotMemberRoom));
                return;
            }

            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await roomValidator.CommandAccessCheckAsync(user, acceptableRoles, room.RoomName))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            var mutedUsers = await mutedUserRepository.GetAsync(new MutedUserModel { UserId = idProcessedUser });
            var idMutedRooms = mutedUsers.Select(m => m.RoomId);

            if (idMutedRooms.Contains(room.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserStillMuted));
                return;
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command + " -m 10", @"//room\smute\s-l\s\w+\s-m\s(10)$");

            var mutedUser = new MutedUser
            {
                UserId = idProcessedUser,
                RoomId = room.Id,
                DateUnmute = dateUnmute
            };

            await mutedUserRepository.AddAsync(mutedUser);

            await clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", room.Id);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenMuted));
        }

        /// <summary>
        /// Временное приглушение пользователя.
        /// Метод добавляет запись с информацией о временном муте пользователя.
        /// </summary>
        private async Task TemporaryMuteUserAsync(string command)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\smute\s-l\s(\w+)\s-m\s\d*$").Groups[1].Value;
            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
                return;
            }

            if (await userValidator.IsUserInGroupByIdAsync(idProcessedUser, room.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserIsNotMemberRoom));
                return;
            }

            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await roomValidator.CommandAccessCheckAsync(user, acceptableRoles, room.RoomName))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            var mutedUsers = await mutedUserRepository.GetAsync(new MutedUserModel { UserId = idProcessedUser });
            var idMutedRooms = mutedUsers.Select(m => m.RoomId);

            if (idMutedRooms.Contains(room.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserStillMuted));
                return;
            }

            DateTime dateUnmute = TimeComputer.CalculateUnlockDate(command, @"//room\smute\s-l\s\w+\s-m\s(\d*)$");

            var mutedUser = new MutedUser
            {
                UserId = idProcessedUser,
                RoomId = room.Id,
                DateUnmute = dateUnmute
            };

            await mutedUserRepository.AddAsync(mutedUser);

            await clients.User(idProcessedUser.ToString()).SendAsync("MuteUser", room.Id);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenMuted));
        }

        /// <summary>
        /// Метод удаляет запись из таблицы о пользователе который был приглушен
        /// </summary>
        private async Task UnmuteUserAsync(string command)
        {
            string userNameProcessedUser = Regex.Match(command, @"//room\sspeak\s-l\s(\w+)$").Groups[1].Value;
            string idProcessedUser = (await userManager.FindByNameAsync(userNameProcessedUser))?.Id;

            if (idProcessedUser == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
                return;
            }

            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await roomValidator.CommandAccessCheckAsync(user, acceptableRoles, room.RoomName))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            if (await userValidator.IsUserInGroupByIdAsync(idProcessedUser, room.Id))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserIsNotMemberRoom));
                return;
            }

            var deleteMutedUser = await mutedUserRepository.GetFirstOrDefaultAsync(new MutedUserModel { UserId = idProcessedUser, RoomId = room.Id });
            // TODO: if deleteMutedUser == null
            await mutedUserRepository.DeleteAsync(deleteMutedUser);

            await clients.User(idProcessedUser.ToString()).SendAsync(
                "UnmutedUser",
                CommandHandler.CreateCommandInfo($"У вас снова есть возможность писать сообщения."),
                room.Id);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserUnmuted));
        }

        // переименовать метод
        private async Task<bool> CheckMainRoomAsync(Room processedRoom)
        {
            if (processedRoom.Id == GlobalConstants.MainRoomId)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.CannotBeAppliedToMainRoom));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет существует ли пользователь к которому применяет команда,
        /// существует ли комната из которой выгоняют.
        /// Состоит ли пользователь в данной группе.
        /// </summary>
        private async Task<string> CheckDataForKickedUserAsync(string nameProcessedRoom, string userNameProcessedUser, string idProcessedRoom, string idProcessedUser)
        {
            if (idProcessedRoom == null)
            {
                return InformationMessages.RoomNotFound;
            }

            if (idProcessedUser == null)
            {
                return InformationMessages.UserNotFound;
            }

            if (await userValidator.IsUserInGroupByIdAsync(idProcessedUser, idProcessedRoom))
            {
                return InformationMessages.UserIsNotMemberRoom;
            }

            return string.Empty;
        }
    }
}