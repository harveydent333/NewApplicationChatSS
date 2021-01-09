using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Models;

namespace NewAppChatSS.BLL.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDTO>> GetRoomMessages(string roomId);
    }
}
