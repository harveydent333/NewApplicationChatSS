using AutoMapper;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewAppChatSS.BLL.Services
{
    public sealed class RoomService : IRoomService
    {
        private IUnitOfWork Database { get; set; }
        private readonly IMapper _mapper;

        public RoomService(IUnitOfWork uow, IMapper mapper)
        {
            Database = uow;
            _mapper = mapper;
        }

        public IEnumerable<RoomDTO> GetRoomsDTO()
        {
            return _mapper.Map<List<RoomDTO>>(Database.Rooms.GetAll());
        }

        public RoomDTO GetRoomDTO(string id)
        {
            return _mapper.Map<RoomDTO>(Database.Rooms.FindById(id));
        }
    }
}
