﻿using System;
using System.Text.Json;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.BLL.Infrastructure.ModelHandlers
{
    /// <summary>
    /// Обработчик сообщений в чате
    /// </summary>
    public class MessageHandler : IMessageHandler
    {
        private readonly IRoomRepository roomRepository;
        private readonly IMessageRepository messageRepository;

        public MessageHandler(
            IMessageRepository messageRepository,
            IRoomRepository roomRepository)
        {
            this.messageRepository = messageRepository;
            this.roomRepository = roomRepository;
        }

        /// <summary>
        /// Метод сохраняет в базу данных информацию о сообщении написаным в пользователем в чат комнаты
        /// </summary>
        public async Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room)
        {
            string messageId = NewAppChatGuidHelper.GetNewGuid();

            var message = new Message
            {
                Id = messageId,
                ContentMessage = textMessage,
                DatePublication = DateTime.Now,
                UserId = user.Id,
                RoomId = room.Id
            };

            await messageRepository.AddAsync(message);

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
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { Ids = new[] { roomId } });

            room.LastMessageId = messageId;
            await roomRepository.ModifyAsync(room);
        }
    }
}
