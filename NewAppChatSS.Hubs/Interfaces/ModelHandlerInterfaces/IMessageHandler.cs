using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces
{
    public interface IMessageHandler
    {
        Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room);
    }
}