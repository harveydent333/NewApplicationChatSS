using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

namespace NewAppChatSS.Hubs.YouTubeAPI
{
    public class YouTubeRequest
    {
        private static string ViewCount { get; set; }

        private static string LikeCount { get; set; }

        private static string ApiKey { get; set; }

        /// <summary>
        /// Метод получает API KEI из файла JSON
        /// </summary>
        public static void GetApiKey()
        {
            using (StreamReader r = new StreamReader("client_secrets.json"))
            {
                string json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<Secret>(json);
                ApiKey = items.API_KEY;
            }
        }

        /// <summary>
        /// Метод подтверждает API KEI для выполнения запросов
        /// </summary>
        public static YouTubeService GetYouTubeService()
        {
            return new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey,
            });
        }

        /// <summary>
        /// Метод выполняет запрос на поиск Ютюб-канал по его названию
        /// </summary>
        public static ChannelListResponse FindYoutubeChannel(YouTubeService youtubeService, string nameChannel)
        {
            ChannelsResource.ListRequest channelRequest = youtubeService.Channels.List("contentDetails, statistics");
            channelRequest.ForUsername = nameChannel;
            channelRequest.MaxResults = 1;
            return channelRequest.Execute();
        }

        /// <summary>
        /// Метод выполняет запрос на поиск видео с ютюб-канала
        /// </summary>
        public static SearchListResponse FindVideoFromChannel(YouTubeService youtubeService, ChannelListResponse channelsListResponse, string nameVideo)
        {
            SearchResource.ListRequest videoList = youtubeService.Search.List("snippet");
            videoList.ChannelId = channelsListResponse.Items[0].Id;
            videoList.MaxResults = 50;
            videoList.Q = nameVideo;
            return videoList.Execute();
        }

        /// <summary>
        /// Метод выполняет запрос на поиск комментариев под видео
        /// </summary>
        public static CommentThreadListResponse FindCommentOnVideo(YouTubeService youtubeService, SearchListResponse videoListResponse)
        {
            CommentThreadsResource.ListRequest commentThreadsRequest = youtubeService.CommentThreads.List("snippet");
            commentThreadsRequest.VideoId = videoListResponse?.Items[0]?.Id.VideoId;
            commentThreadsRequest.MaxResults = 25;
            return commentThreadsRequest.Execute();
        }

        /// <summary>
        /// Метод возвращает информацию при пустом результате поиска ютюб-канала
        /// </summary>
        public static string GetInformationAboutEmptyChannel(string nameChannel)
        {
            return System.Text.Json.JsonSerializer.Serialize<object>(
                new
                {
                    datePublication = DateTime.Now,
                    title = $"Канал с названием {nameChannel} не найден",
                    view = ViewCount,
                    likes = LikeCount,
                });
        }

        /// <summary>
        /// Метод возвращает информацию при отсутствии видео на ютюб-канале
        /// </summary>
        public static string GetInformationAboutEmptySearch(string nameChannel)
        {
            return System.Text.Json.JsonSerializer.Serialize<object>(
                new
                {
                    datePublication = DateTime.Now,
                    title = $"На канале {nameChannel} не найдено данного видео",
                    view = ViewCount,
                    likes = LikeCount,
                });
        }

        /// <summary>
        /// Метод возвращает информацию при пустом результате при поиске комменатириев под видео
        /// </summary>
        public static string GetInformationAboutEmptyComment(string nameVideo)
        {
            return System.Text.Json.JsonSerializer.Serialize<object>(
                new
                {
                    datePublication = DateTime.Now,
                    title = $"Под видео {nameVideo} нет ни одного комментария",
                });
        }

        public static SearchListResponse GetVideosFromYouTubeChannel(YouTubeService youtubeService, ChannelListResponse channelsListResponse)
        {
            SearchResource.ListRequest videoList = youtubeService.Search.List("snippet");
            videoList.ChannelId = channelsListResponse.Items[0].Id;
            videoList.MaxResults = 10;
            videoList.Order = SearchResource.ListRequest.OrderEnum.Date;
            return videoList.Execute();
        }

        /// <summary>
        /// Метод возвращает 5 ссылок на видео ролики, содержащиеся на ютюб-канале
        /// </summary>
        public static string GetChannelInfo(string nameChannel)
        {
            GetApiKey();
            YouTubeService youtubeService = GetYouTubeService();
            ChannelListResponse channelsListResponse = FindYoutubeChannel(youtubeService, nameChannel);

            if (channelsListResponse.Items == null)
            {
                return GetInformationAboutEmptyChannel(nameChannel);
            }

            var videoListResponse = GetVideosFromYouTubeChannel(youtubeService, channelsListResponse);

            if (videoListResponse.Items.Count == 0)
            {
                return GetInformationAboutEmptySearch(nameChannel);
            }

            List<string> referencesOnVideos = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                if (videoListResponse.Items[i] != null)
                {
                    referencesOnVideos.Add("https://www.youtube.com/watch?v=" + videoListResponse.Items[i].Id.VideoId);
                }
            }

            return System.Text.Json.JsonSerializer.Serialize<object>(
                new
                {
                    datePublication = DateTime.Now,
                    title = nameChannel,
                    messageContent = referencesOnVideos,
                });
        }

        /// <summary>
        /// Метод возвращает ссылку на видео, а также информацию о лайках и просмотром искомого по названию канала и видео
        /// </summary>
        public static string GetRefOnVideo(string nameChannel, string nameVideo, bool views = false, bool likes = false)
        {
            GetApiKey();
            YouTubeService youtubeService = GetYouTubeService();
            ChannelListResponse channelsListResponse = FindYoutubeChannel(youtubeService, nameChannel);

            if (channelsListResponse.Items == null)
            {
                return GetInformationAboutEmptyChannel(nameChannel);
            }

            SearchListResponse videoListResponse = FindVideoFromChannel(youtubeService, channelsListResponse, nameVideo);

            if (videoListResponse.Items.Count == 0)
            {
                return GetInformationAboutEmptySearch(nameChannel);
            }

            var videoInfo = youtubeService.Videos.List("statistics,snippet");
            videoInfo.Id = videoListResponse?.Items[0]?.Id.VideoId;
            var videoInfoResponse = videoInfo.Execute();

            if (views)
            {
                ViewCount = videoInfoResponse.Items[0].Statistics.ViewCount.ToString();
            }

            if (likes)
            {
                LikeCount = videoInfoResponse.Items[0].Statistics.LikeCount.ToString();
            }

            return System.Text.Json.JsonSerializer.Serialize<object>(
                new
                {
                    datePublication = DateTime.Now,
                    title = "https://www.youtube.com/watch?v=" + videoListResponse?.Items[0]?.Id.VideoId,
                    view = ViewCount,
                    likes = LikeCount,
                });
        }

        /// <summary>
        /// Метод возвращает рандомный комментарий под видео
        /// </summary>
        public static string GetCommentVideo(string nameChannel, string nameVideo)
        {
            GetApiKey();
            YouTubeService youtubeService = GetYouTubeService();
            ChannelListResponse channelsListResponse = FindYoutubeChannel(youtubeService, nameChannel);

            if (channelsListResponse.Items == null)
            {
                return GetInformationAboutEmptyChannel(nameChannel);
            }

            SearchListResponse videoListResponse = FindVideoFromChannel(youtubeService, channelsListResponse, nameVideo);

            if (videoListResponse.Items.Count == 0)
            {
                return GetInformationAboutEmptySearch(nameChannel);
            }

            CommentThreadListResponse commentResponse = FindCommentOnVideo(youtubeService, videoListResponse);

            if (commentResponse.Items.Count == 0)
            {
                return GetInformationAboutEmptyComment(nameVideo);
            }

            int commentIndex = new Random().Next(0, commentResponse.Items.Count);

            //TODO: Вынести
            var commentInfo = new
            {
                datePublication = DateTime.Now,
                authorComment = commentResponse.Items[commentIndex].Snippet.TopLevelComment.Snippet.AuthorDisplayName,
                commentContext = commentResponse.Items[commentIndex].Snippet.TopLevelComment.Snippet.TextDisplay,
            };

            return System.Text.Json.JsonSerializer.Serialize<object>(commentInfo);
        }

        /// <summary>
        /// Метод авторизирует профиль в ютюбе.
        /// </summary>
        public async Task Run()
        {
            ApiKey = "";
            ViewCount = "";
            LikeCount = "";
            UserCredential credential;

            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "admin",
                    CancellationToken.None,
                    new FileDataStore(GetType().ToString()));
            }
        }
    }
}
