using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces
{
    public interface IMemberService
    {
        Task<List<RoomDTO>> GetUserRoomsAsync(string userId);
    }
}
