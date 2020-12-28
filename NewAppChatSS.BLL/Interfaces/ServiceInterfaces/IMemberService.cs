using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMemberService
    {
        List<RoomDTO> GetRoomsUser(string userId);
    }
}
