using System.Collections.Generic;
using AutoMapper;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Services
{
    public sealed class MessageService : IMessageService
    {
        public IUnitOfWork Database { get; set; }

        private readonly IMapper mapper;

        public MessageService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            this.mapper = mapper;
        }

        public List<MessageDTO> GetRoomMessagesDTO(string roomId)
        {
            return mapper.Map<List<MessageDTO>>(Database.Messages.GetRoomMessages(roomId));
        }
    }
}
