using System.Threading.Tasks;

namespace NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces
{
    public interface IRoomHandler
    {
        Task<string> CreateRoomAsync(string roomName, int typeRoomId, string userId);
    }
}
