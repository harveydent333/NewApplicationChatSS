using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces
{
    public interface IMessageHandler
    {
        Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room);
    }
}