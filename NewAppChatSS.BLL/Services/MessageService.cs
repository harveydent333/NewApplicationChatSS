using AutoMapper;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Services
{
    public sealed class MessageService : IMessageService
    {
        public IUnitOfWork Database { get; set; }
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            _mapper = mapper;
        }

        public IEnumerable<MessageDTO> GetRoomMessagesDTO(string roomId)
        {
            return _mapper.Map<IEnumerable<MessageDTO>>(Database.Messages.GetRoomMessages(roomId));
        }
    }
}
