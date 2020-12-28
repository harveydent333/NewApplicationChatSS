using System.Collections.Generic;
using AutoMapper;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Services
{
    public sealed class RoomService : IRoomService
    {
        private IUnitOfWork Database { get; set; }

        private readonly IMapper mapper;

        public RoomService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            this.mapper = mapper;
        }

        public List<RoomDTO> GetRooms()
        {
            return mapper.Map<List<RoomDTO>>(Database.Rooms.GetAll());
        }

        public RoomDTO GetRoom(string id)
        {
            return mapper.Map<RoomDTO>(Database.Rooms.FindById(id));
        }
    }
}
