using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces
{
    public interface IRoomHandler
    {
        string CreateRoom(string roomName, int typeRoomId, string userId);
    }
}
