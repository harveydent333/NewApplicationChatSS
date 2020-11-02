using NewAppChatSS.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        IEnumerable<MessageDTO> GetRoomMessagesDTO(string roomId);
    }
}
