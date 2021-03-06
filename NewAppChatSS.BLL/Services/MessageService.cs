﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.BLL.Services
{
    public sealed class MessageService : IMessageService
    {
        private readonly IMapper mapper;
        private readonly IMessageRepository messageRepository;

        public MessageService(
            IMessageRepository messageRepository,
            IMapper mapper)
        {
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        public async Task<List<MessageDTO>> GetRoomMessagesAsync(string roomId)
        {
            var messageModel = new MessageModel { IncludeRoom = true, IncludeUser = true, RoomId = roomId };
            var messages = (await messageRepository.GetAsync(messageModel))
                .OrderBy(m => m.DatePublication).ToList();

            return mapper.Map<List<MessageDTO>>(messages);
        }
    }
}