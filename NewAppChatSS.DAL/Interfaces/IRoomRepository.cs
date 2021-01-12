using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IRoomRepository : IBaseRepository<Room, string, NewAppChatSSContext, RoomModel>
    {
    }
}