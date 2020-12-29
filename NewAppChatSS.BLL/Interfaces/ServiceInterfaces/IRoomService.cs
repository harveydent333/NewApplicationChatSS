using System.Collections.Generic;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IRoomService
    {
        List<RoomDTO> GetRooms();

        RoomDTO GetRoom(string id);
    }
}
