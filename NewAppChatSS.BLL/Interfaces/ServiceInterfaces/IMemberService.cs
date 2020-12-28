using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMemberService
    {
        IEnumerable<RoomDTO> GetRoomsUser(string userId);
    }
}
