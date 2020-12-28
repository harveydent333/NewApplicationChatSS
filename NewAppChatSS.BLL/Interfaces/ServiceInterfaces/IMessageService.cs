﻿using System.Collections.Generic;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        List<MessageDTO> GetRoomMessages(string roomId);
    }
}
