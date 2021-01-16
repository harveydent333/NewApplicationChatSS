using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    public class CommandHandler
    {
        /// <summary>
        /// Метод формирует JSON объект c информацией о команде для отправки клиенту
        /// </summary>
        /// <param name="messageText">Текст с </param>
        /// <returns>Возвращает сериализованную строку ответа</returns>
        public static string CreateResponseMessage(string messageText)
        {
            var responseMessage = new
            {
                messageText = messageText,
                datePublication = DateTime.Now,
            };

            return JsonSerializer.Serialize<object>(responseMessage);
        }

        /// <summary>
        /// Метод формирует JSON объект c информацией о команде для отправки клиенту
        /// </summary>
        public static string CreateCommandInfo(List<string> allowedCommands)
        {
            var commandsInfo = new
            {
                messageContent = allowedCommands,
                datePublication = DateTime.Now,
            };

            return JsonSerializer.Serialize<object>(commandsInfo);
        }
    }
}
