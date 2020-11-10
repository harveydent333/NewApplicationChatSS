using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Infrastructure.YouTube_API;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Hubs.CommandHandlersHubs
{
    public sealed class BotCommandHandlerHub : IBotCommandHandlerHub
    {
        private readonly Dictionary<Regex, Func<string, IHubCallerClients, Task>> botCommands;

        public BotCommandHandlerHub()
        {
            botCommands = new Dictionary<Regex, Func<string, IHubCallerClients, Task>>
            {
                [new Regex(@"^//find\s[\w\s]+\W{2}.+[^-v|^-l]$")] = CreateRefOnVideo,
                [new Regex(@"^//find\s[\s\w\d]+\W{2}.+[^-l]\s-v$")] = CreateRefOnVideoAndGetCountViews,
                [new Regex(@"^//find\s[\s\w\d]+\W{2}.+[^-v]\s-l$")] = CreateRefOnVideoAndGetCountLikes,
                [new Regex(@"^//find\s[\s\w\d]+\W{2}.+(\s-v\s-l|\s-l\s-v)$")] = CreateRefOnVideoAndGetCountViewsAndLikes,
                [new Regex(@"^//info\s[\s\w\d]+$")] = GetInfoAboutChannel,
                [new Regex(@"^//videoCommentRandom\s[\s\d\w]+\W\W.+$")] = GetRandomComments,
            };
        }

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommand(string command, IHubCallerClients calledClients)
        {
            foreach (Regex keyCommand in botCommands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    return botCommands[keyCommand](command, calledClients);
                }
            }

            return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео 
        /// </summary>
        private async Task<Task> CreateRefOnVideo(string command, IHubCallerClients clients)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            YouTubeRequest youTubeRequest = new YouTubeRequest();
            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров под искомым видео
        /// </summary>
        private async Task<Task> CreateRefOnVideoAndGetCountViews(string command, IHubCallerClients clients)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-v", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\w\W]+)$").Groups[2].Value;

            YouTubeRequest youTubeRequest = new YouTubeRequest();
            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве лайков под искомым видео
        /// </summary>
        private async Task<Task> CreateRefOnVideoAndGetCountLikes(string command, IHubCallerClients clients)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w])+$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-l", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            YouTubeRequest youTubeRequest = new YouTubeRequest();
            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, false, true);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров и лайков под искомым видео
        /// </summary>
        private async Task<Task> CreateRefOnVideoAndGetCountViewsAndLikes(string command, IHubCallerClients clients)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-l", "");
            command = Regex.Replace(command, @"\s-v", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            YouTubeRequest youTubeRequest = new YouTubeRequest();
            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true, true);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылки к 5 видео, имеющиеся на канале
        /// </summary>
        private async Task<Task> GetInfoAboutChannel(string command, IHubCallerClients clients)
        {
            string nameChannel = Regex.Match(command, @"//info\s([\s\W\w]+)$").Groups[1].Value;

            YouTubeRequest youTubeRequest = new YouTubeRequest();
            await youTubeRequest.Run();

            return clients.Caller.SendAsync("PrintInfoAboutChannel", YouTubeRequest.GetChannelInfo(nameChannel));
        }

        /// <summary>
        /// Метод возвращает информацию о комментарии к видео, принимая название канала и видео
        /// </summary>
        private Task GetRandomComments(string command, IHubCallerClients clients)
        {
            string nameChannel = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            string nameVideo = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            return clients.Caller.SendAsync("PrintCommentInfo",
                YouTubeRequest.GetCommentVideo(nameChannel, nameVideo));
        }
    }
}
