using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.Hubs.Interfaces.HubInterfaces;
using NewAppChatSS.Hubs.YouTubeAPI;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    public sealed class BotCommandHandlerHub : IBotCommandHandlerHub
    {
        private readonly Dictionary<Regex, Func<string, Task>> botCommands;
        private readonly YouTubeRequest youTubeRequest;

        private IHubCallerClients clients;

        public BotCommandHandlerHub(YouTubeRequest youTubeRequest)
        {
            this.youTubeRequest = youTubeRequest;

            botCommands = new Dictionary<Regex, Func<string, Task>>
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
        public Task SearchCommand(string command, IHubCallerClients clients)
        {
            this.clients = clients;

            foreach (Regex keyCommand in botCommands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    return botCommands[keyCommand](command);
                }
            }

            return clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.IncorrectCommand));
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео
        /// </summary>
        private async Task<Task> CreateRefOnVideo(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров под искомым видео
        /// </summary>
        private async Task<Task> CreateRefOnVideoAndGetCountViews(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-v", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\w\W]+)$").Groups[2].Value;

            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве лайков под искомым видео
        /// </summary>
        private async Task<Task> CreateRefOnVideoAndGetCountLikes(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w])+$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-l", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, false, true);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров и лайков под искомым видео
        /// </summary>
        private async Task<Task> CreateRefOnVideoAndGetCountViewsAndLikes(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-l", "");
            command = Regex.Replace(command, @"\s-v", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await youTubeRequest.Run();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true, true);

            return clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылки к 5 видео, имеющиеся на канале
        /// </summary>
        private async Task<Task> GetInfoAboutChannel(string command)
        {
            string nameChannel = Regex.Match(command, @"//info\s([\s\W\w]+)$").Groups[1].Value;

            await youTubeRequest.Run();

            return clients.Caller.SendAsync("PrintInfoAboutChannel", YouTubeRequest.GetChannelInfo(nameChannel));
        }

        /// <summary>
        /// Метод возвращает информацию о комментарии к видео, принимая название канала и видео
        /// </summary>
        private Task GetRandomComments(string command)
        {
            string nameChannel = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            string nameVideo = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            return clients.Caller.SendAsync(
                "PrintCommentInfo", YouTubeRequest.GetCommentVideo(nameChannel, nameVideo));
        }
    }
}
