using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
        public async Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room)
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

            await Database.Messages.AddMessage(message);

            await AddInfoAboutLastMessage(messageId, room.Id);

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
        private async Task AddInfoAboutLastMessage(string messageId, string roomId)
        {
            Room room = Database.Rooms.FindById(roomId);

            room.LastMessageId = messageId;
            await Database.Rooms.UpdateAsync(room);
        }

        /// <summary>
        /// Метод добавляет информацию о ругающемся пользователе в базу данных
        /// </summary>
        private void HandleSwearingUser(string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод проверяет содержатся ли ругательские слова в тексте сообщения
        /// </summary>
        private bool ContainsSwearWords(string textMessage)
        {
            throw new NotImplementedException();
        }
    }
}
