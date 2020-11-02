using AutoMapper;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Services
{
    public class MemberService : IMemberService
    {
        public IUnitOfWork Database { get; set; }
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            _mapper = mapper;
        }

        public IEnumerable<RoomDTO> GetRoomsUser(string userId)
        {
            return _mapper.Map<IEnumerable<RoomDTO>>(Database.Members.GetRooms(userId));
        }
    }
}
