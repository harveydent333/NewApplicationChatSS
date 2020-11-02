using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace NewAppChatSS.BLL.Infrastructure.ModelHandlers
{
    /// <summary>
    /// Обработчик сообщений в чате
    /// </summary>
    public class MessageHandler : IMessageHandler
    {
        public IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;

        public MessageHandler(IUnitOfWork uow, UserManager<User> userManager)
        {
            _userManager = userManager;
            Database = uow;
        }

        /// <summary>
        /// Метод сохраняет в базу данных информацию о сообщении написаным в пользователем в чат комнаты
        /// </summary>
        public string SaveMessageIntoDatabase(User user, string textMessage, Room room)
        {
            string messageId = Guid.NewGuid().ToString();

            Message message = new Message
            {
                Id = messageId,
                ContentMessage = textMessage,
                DatePublication = DateTime.Now,
                UserId = user.Id,
                RoomId = room.Id
            };

            Database.Messages.AddMessage(message);

            AddInfoAboutLastMessage(messageId, room.Id);

            return JsonSerializer.Serialize<object>(new
            {
                userName = user.UserName,
                messageId = message.Id,
                messageContent = message.ContentMessage,
                datePublication = DateTime.Now,
                roomId = room.Id
            });
        }

        /// <summary>
        /// Метод добавляет информацию в базу данных о последем сообщении в комнате
        /// </summary>
        public void AddInfoAboutLastMessage(string messageId, string roomId)
        {
            Room room = Database.Rooms.FindById(roomId);

            room.LastMessageId = messageId;
            Database.Rooms.Update(room);
        }

        /// <summary>
        /// Метод добавляет информацию о ругающемся пользователе в базу данных
        /// </summary>
        public void HandleSwearingUser(string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод проверяет содержатся ли ругательские слова в тексте сообщения
        /// </summary>
        public static bool ContainsSwearWords(string textMessage)
        {
            throw new NotImplementedException();
        }
    }
}
