using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces
{
    public interface IRoomHandler
    {
        Task<string> CreateRoom(string roomName, int typeRoomId, string userId);
    }
}
