using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NewAppChatSS.BLL.Infrastructure
{
    public class CommandHandler
    {
        /// <summary>
        ///  Метод формирует JSON объект c информацией о команде для отправки клиенту
        /// </summary>
        public static string CreateCommandInfo(string textCommand)
        {
            return JsonSerializer.Serialize<object>(new
            {
                messageContent = textCommand,
                datePublication = DateTime.Now,
            });
        }

        /// <summary>
        /// Метод формирует JSON объект c информацией о команде для отправки клиенту
        /// </summary>
        public static string CreateCommandInfo(List<string> allowedCommands)
        {
            return JsonSerializer.Serialize<object>(new
            {
                messageContent = allowedCommands,
                datePublication = DateTime.Now,
            });
        }
    }
}
