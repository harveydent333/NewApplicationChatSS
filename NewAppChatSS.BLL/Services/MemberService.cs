using System.Collections.Generic;
using AutoMapper;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Services
{
    public sealed class MemberService : IMemberService
    {
        public IUnitOfWork Database { get; set; }

        private readonly IMapper mapper;

        public MemberService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            this.mapper = mapper;
        }

        public List<RoomDTO> GetUserRooms(string userId)
        {
            return mapper.Map<List<RoomDTO>>(Database.Members.GetRooms(userId));
        }
    }
}
