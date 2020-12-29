using System;
using System.Text.Json;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Infrastructure.ModelHandlers
{
    /// <summary>
    /// Обработчик сообщений в чате
    /// </summary>
    public class MessageHandler : IMessageHandler
    {
        public MessageHandler(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IUnitOfWork Database { get; set; }

        /// <summary>
        /// Метод сохраняет в базу данных информацию о сообщении написаным в пользователем в чат комнаты
        /// </summary>
        public async Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room)
        {
            string messageId = NewAppChatGuidHelper.GetNewGuid();

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

            var messageInfo = new
            {
                userName = user.UserName,
                messageId = message.Id,
                messageContent = message.ContentMessage,
                datePublication = DateTime.Now,
                roomId = room.Id
            };

            return JsonSerializer.Serialize<object>(messageInfo);
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
    }
}
