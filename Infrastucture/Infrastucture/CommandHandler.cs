using System;
using System.Collections.Generic;
using System.Text.Json;
namespace AppChatSS.Infrastucture
{
    public class CommandHandler
    {
        /// <summary>
        ///  Метод формирует JSON объект c информацией о команде для отправки клиенту
        /// </summary>
        public static String CreateCommandInfo(String textCommand)
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
        public static String CreateCommandInfo(List<String> allowedCommands)
        {
            return JsonSerializer.Serialize<object>(new
            {
                messageContent = allowedCommands,
                datePublication = DateTime.Now,
            });
        }
    }
}
