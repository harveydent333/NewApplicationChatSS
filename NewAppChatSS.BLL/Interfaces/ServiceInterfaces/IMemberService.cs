using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMemberService
    {
        Task<List<RoomDTO>> GetUserRooms(string userId);
    }
}
