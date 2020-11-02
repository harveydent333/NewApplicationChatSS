using NewAppChatSS.BLL.DTO;
using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMemberService
    {
        IEnumerable<RoomDTO> GetRoomsUser(string userId);
    }
}
