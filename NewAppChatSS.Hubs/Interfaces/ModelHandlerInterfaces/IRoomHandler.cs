using System.Threading.Tasks;

namespace NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces
{
    public interface IRoomHandler
    {
        Task<string> CreateRoom(string roomName, int typeRoomId, string userId);
    }
}
