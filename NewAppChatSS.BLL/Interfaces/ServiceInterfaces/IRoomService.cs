using System.Threading.Tasks;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IRoomService
    {
        Task<RoomDTO> GetRoom(string id);
    }
}
