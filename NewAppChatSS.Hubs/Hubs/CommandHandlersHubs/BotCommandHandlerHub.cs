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
    public sealed class BotCommandHandlerHub : AbstarctHub, IBotCommandHandlerHub
    {
        private readonly YouTubeRequest youTubeRequest;

        public override Dictionary<Regex, Func<string, Task>> Commands { get; }

        public BotCommandHandlerHub(YouTubeRequest youTubeRequest)
        {
            this.youTubeRequest = youTubeRequest;

            Commands = new Dictionary<Regex, Func<string, Task>>
            {
                [new Regex(@"^//find\s[\w\s]+\W{2}.+[^-v|^-l]$")] = CreateRefOnVideoAsync,
                [new Regex(@"^//find\s[\s\w\d]+\W{2}.+[^-l]\s-v$")] = CreateRefOnVideoAndGetCountViewsAsync,
                [new Regex(@"^//find\s[\s\w\d]+\W{2}.+[^-v]\s-l$")] = CreateRefOnVideoAndGetCountLikesAsync,
                [new Regex(@"^//find\s[\s\w\d]+\W{2}.+(\s-v\s-l|\s-l\s-v)$")] = CreateRefOnVideoAndGetCountViewsAndLikesAsync,
                [new Regex(@"^//info\s[\s\w\d]+$")] = GetInfoAboutChannelAsync,
                [new Regex(@"^//videoCommentRandom\s[\s\d\w]+\W\W.+$")] = GetRandomCommentsAsync,
            };
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео
        /// </summary>
        private async Task CreateRefOnVideoAsync(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await youTubeRequest.RunAsync();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo);

            await clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров под искомым видео
        /// </summary>
        private async Task CreateRefOnVideoAndGetCountViewsAsync(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-v", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\w\W]+)$").Groups[2].Value;

            await youTubeRequest.RunAsync();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true);

            await clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве лайков под искомым видео
        /// </summary>
        private async Task CreateRefOnVideoAndGetCountLikesAsync(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w])+$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-l", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await youTubeRequest.RunAsync();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, false, true);

            await clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылку на искомое видео и информацию о количестве просмотров и лайков под искомым видео
        /// </summary>
        private async Task CreateRefOnVideoAndGetCountViewsAndLikesAsync(string command)
        {
            string nameChannel = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            command = Regex.Replace(command, @"\s-l", "");
            command = Regex.Replace(command, @"\s-v", "");
            string nameVideo = Regex.Match(command, @"//find\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await youTubeRequest.RunAsync();

            var referenceVideo = YouTubeRequest.GetRefOnVideo(nameChannel, nameVideo, true, true);

            await clients.Caller.SendAsync("PrintInfoAboutVideo", referenceVideo);
        }

        /// <summary>
        /// Метод возвращает ссылки к 5 видео, имеющиеся на канале
        /// </summary>
        private async Task GetInfoAboutChannelAsync(string command)
        {
            string nameChannel = Regex.Match(command, @"//info\s([\s\W\w]+)$").Groups[1].Value;

            await youTubeRequest.RunAsync();
            await clients.Caller.SendAsync("PrintInfoAboutChannel", YouTubeRequest.GetChannelInfo(nameChannel));
        }

        /// <summary>
        /// Метод возвращает информацию о комментарии к видео, принимая название канала и видео
        /// </summary>
        private async Task GetRandomCommentsAsync(string command)
        {
            string nameChannel = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[1].Value;
            string nameVideo = Regex.Match(command, @"//videoCommentRandom\s([\s\w]+)\W{2}([\s\W\w]+)$").Groups[2].Value;

            await clients.Caller.SendAsync("PrintCommentInfo", YouTubeRequest.GetCommentVideo(nameChannel, nameVideo));
        }
    }
}
