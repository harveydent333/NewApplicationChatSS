using System.Collections.Generic;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMemberService
    {
        List<RoomDTO> GetUserRooms(string userId);
    }
}
