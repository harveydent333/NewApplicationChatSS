using NewAppChatSS.BLL.DTO;
using System.Collections.Generic;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        IEnumerable<MessageDTO> GetRoomMessagesDTO(string roomId);
    }
}
