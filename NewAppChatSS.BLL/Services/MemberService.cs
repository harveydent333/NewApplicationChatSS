using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.BLL.Services
{
    public sealed class MemberService : IMemberService
    {
        private readonly IMapper mapper;
        private readonly IMemberRepository memberRepository;
        private readonly IRoomRepository roomRepository;

        public MemberService(
            IRoomRepository roomRepository,
            IMemberRepository memberRepository,
            IMapper mapper)
        {
            this.roomRepository = roomRepository;
            this.memberRepository = memberRepository;
            this.mapper = mapper;
        }

        // TODO: need to improve this method
        public async Task<List<RoomDTO>> GetUserRooms(string userId)
        {
            var roomIds = (await memberRepository.GetAsync(new MemberModel { UserId = userId })).Select(m => m.RoomId);

            var rooms = await roomRepository.GetAsync(new RoomModel { IncludeLastMessage = true, IncludeOwner = true, IncludeTypeRoom = true });
            var userRooms = rooms.Where(r => roomIds.Contains(r.Id)).ToList();

            return mapper.Map<List<RoomDTO>>(userRooms);
        }
    }
}
