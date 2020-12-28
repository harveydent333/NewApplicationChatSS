using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.BLL.DTO;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IRoomService
    {
        IEnumerable<RoomDTO> GetRoomsDTO();

        RoomDTO GetRoomDTO(string id);
    }
}
