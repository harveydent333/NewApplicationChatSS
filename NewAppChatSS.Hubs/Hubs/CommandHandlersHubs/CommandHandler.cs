using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    public class CommandHandler
    {
        /// <summary>
        ///  Метод формирует JSON объект c информацией о команде для отправки клиенту
        /// </summary>
        public static string CreateCommandInfo(string textCommand)
        {
            var commandInfo = new
            {
                messageContent = textCommand,
                datePublication = DateTime.Now,
            };

            return JsonSerializer.Serialize<object>(commandInfo);
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
