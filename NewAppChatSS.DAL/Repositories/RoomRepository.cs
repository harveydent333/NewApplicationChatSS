using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class RoomRepository : BaseRepository<Room, string, NewAppChatSSContext, RoomModel>, IRoomRepository
    {
        public RoomRepository(NewAppChatSSContext context)
            : base(context)
        {
        }
    }
}
