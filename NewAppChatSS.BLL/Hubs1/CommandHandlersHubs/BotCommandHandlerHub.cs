//using AppChatSS.Infrastucture;
//using AppChatSS.Infrastucture.YouTube_API;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace AppChatSS.Hubs.CommandHandlersHubs
//{
//    /// <summary>
//    /// Обработчик команд взаимодействия с чат-бот комнатой
//    /// </summary>
//    public class BotCommandHandlerHub
//    {
//        private static Dictionary<Regex, Func<String, IHubCallerClients, Task>> botCommands = new Dictionary<Regex, Func<String, IHubCallerClients, Task>>
//        {
//            [new Regex(@"^//find\s[\w\s]+\W{2}.+[^-v|^-l]$")] = CreateRefOnVideo,
//            [new Regex(@"^//find\s[\s\w\d]+\W{2}.+[^-l]\s-v$")] = CreateRefOnVideoAndGetCountViews,
//            [new Regex(@"^//find\s[\s\w\d]+\W{2}.+[^-v]\s-l$")] = CreateRefOnVideoAndGetCountLikes,
//            [new Regex(@"^//find\s[\s\w\d]+\W{2}.+(\s-v\s-l|\s-l\s-v)$")] = CreateRefOnVideoAndGetCountViewsAndLikes,
//            [new Regex(@"^//info\s[\s\w\d]+$")] = GetInfoAboutChannel,
//            [new Regex(@"^//videoCommentRandom\s[\s\d\w]+\W\W.+$")] = GetRandomComments,
//        };

//        /// <summary>
//        /// Метод проверяет какое регулярное выражение соответствует полученной команде
//        /// по результатам перенаправляет на нужный метод обработки команды
//        /// </summary>
//        public static Task SearchCommand(String command, IHubCallerClients calledClients)
//        {
//            foreach (Regex keyCommand in botCommands.Keys)
//            {
//                if (keyCommand.IsMatch(command))
//                {
//                    return botCommands[keyCommand](command, calledClients);
//                }
//            }

//            return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
//        }

//        /// <summary>
//        /// Метод возвращает ссылку на искомое видео 
//        /// </summary>
//        public static Task CreateRefOnVideo(String command, IHubCallerClients clients)
//        {
//            String nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
//            String nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

//            YouTubeRequest youTubeRequest = new YouTubeRequest();
//            youTubeRequest.Run();

//            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo);

//            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
//        }

//        /// <summary>
//        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров под искомым видео
//        /// </summary>
//        public static Task CreateRefOnVideoAndGetCountViews(String command, IHubCallerClients clients)
//        {
//            String nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
//            command = Regex.Replace(command, @"\s-v", "");
//            String nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\w\W]+)$").Groups[2].Value;

//            YouTubeRequest youTubeRequest = new YouTubeRequest();
//            youTubeRequest.Run();

//            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true);

//            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
//        }

//        /// <summary>
//        /// Метод возвращает ссылку на искомое видео и информацию о количестве лайков под искомым видео
//        /// </summary>
//        public static Task CreateRefOnVideoAndGetCountLikes(String command, IHubCallerClients clients)
//        {
//            String nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w])+$").Groups[1].Value;
//            command = Regex.Replace(command, @"\s-l", "");
//            String nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

//            YouTubeRequest youTubeRequest = new YouTubeRequest();
//            youTubeRequest.Run();

//            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, false, true);

//            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
//        }

//        /// <summary>
//        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров и лайков под искомым видео
//        /// </summary>
//        public static Task CreateRefOnVideoAndGetCountViewsAndLikes(String command, IHubCallerClients clients)
//        {
//            String nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
//            command = Regex.Replace(command, @"\s-l", "");
//            command = Regex.Replace(command, @"\s-v", "");
//            String nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

//            YouTubeRequest youTubeRequest = new YouTubeRequest();
//            youTubeRequest.Run();

//            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true, true);

//            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
//        }

//        /// <summary>
//        /// Метод возвращает ссылки к 5 видео, имеющиеся на канале
//        /// </summary>
//        public static Task GetInfoAboutChannel(String command, IHubCallerClients clients)
//        {
//            String nameChannel = Regex.Match(command, @"//info\s([\s\W\w]+)$").Groups[1].Value;

//            YouTubeRequest youTubeRequest = new YouTubeRequest();
//            youTubeRequest.Run();

//            return clients.Caller.SendAsync("PrintInfoAboutChannel", YouTubeRequest.GetChannelInfo(nameChannel));
//        }

//        /// <summary>
//        /// Метод возвращает информацию о комментарии к видео, принимая название канала и видео
//        /// </summary>
//        public static Task GetRandomComments(String command, IHubCallerClients clients)
//        {
//            String nameChannel = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
//            String nameVideo = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

//            return clients.Caller.SendAsync("PrintCommentInfo",
//                YouTubeRequest.GetCommentVideo(nameChannel, nameVideo));
//        }
//    }
//}

