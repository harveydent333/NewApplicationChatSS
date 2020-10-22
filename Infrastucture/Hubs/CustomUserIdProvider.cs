﻿using Microsoft.AspNetCore.SignalR;

namespace AppChatSS.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Метод позвращает ConnectionId по имени авторизированного пользователя
        /// </summary>
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity.Name;
        }
    }
}
