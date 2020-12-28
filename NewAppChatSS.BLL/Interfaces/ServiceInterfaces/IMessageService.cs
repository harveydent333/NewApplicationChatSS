using System.Collections.Generic;
using NewAppChatSS.BLL.DTO;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        IEnumerable<MessageDTO> GetRoomMessagesDTO(string roomId);
    }
}
