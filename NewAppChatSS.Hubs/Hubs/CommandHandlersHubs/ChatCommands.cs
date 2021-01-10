using System;
using System.Collections.Generic;
using System.Text;

// TODO: Вынести в отдельную папку и разбить по файлам (если будут новые команды), пользователь-комната-бот
namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    public class ChatCommands
    {
        public const string UserRename = "//user rename {login пользователя}||{Новый login пользователя}";
        public const string UserBan = "//user ban {login пользователя} [-m {Количество минут}]";
        public const string UserUnban = "//user pardon {login пользователя}";
        public const string UserMute = "//room mute -l {login пользователя} [-m {Количество минут}]";
        public const string UserUnmute = "//room speak -l {login пользователя}";
        public const string UserKick = "//room disconnect {Название комнаты} [-l {login пользователя}] [-m {Количество минут}]";
        public const string ChangeModeratorRole = "//user moderator {login пользователя} [-n] or [-d]";

        public const string RoomRename = "//room rename {Название комнаты}";
        public const string RoomCreate = "//room create {Название комнаты} [-c] or [-b]";
        public const string RoomRemove = "//room remove {Название комнаты}";
        public const string DisconnectFromCurrentRoom = "//room disconnect";
        public const string DisconeectFromRoom = "//room disconnect {Название комнаты}";
        public const string ConnectToRoom = "//room connect {Название комнаты} -l {login пользователя}";

        public const string FindVideoOnYouTubeChannel = "//find {название канала}||{название видео} [-v] [-l]";
        public const string GetInfoAboutYouTubeChannel = "//info {название канала}";
        public const string GetVideoComments = "//videoCommentRandom {название канала}||{Название ролика}";
    }
}
