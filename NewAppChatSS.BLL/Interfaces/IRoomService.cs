using System.Threading.Tasks;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces
{
    public interface IRoomService
    {
        Task<RoomDTO> GetRoomAsync(string id);
    }
}
