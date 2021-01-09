using System.Threading.Tasks;
using AutoMapper;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.BLL.Services
{
    public sealed class RoomService : IRoomService
    {
        private readonly IRoomRepository roomRepository;
        private readonly IMapper mapper;

        public RoomService(
            IRoomRepository roomRepository,
            IMapper mapper)
        {
            this.roomRepository = roomRepository;
            this.mapper = mapper;
        }

        public async Task<RoomDTO> GetRoom(string id)
        {
            var room = await roomRepository.GetFirstOrDefaultAsync(
                new RoomModel
                {
                    Ids = new[] { id },
                    IncludeTypeRoom = true,
                    IncludeOwner = true,
                    IncludeLastMessage = true
                });

            return mapper.Map<RoomDTO>(room);
        }
    }
}
