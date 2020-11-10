using NewAppChatSS.DAL.Entities;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces
{
    public interface IMessageHandler
    {
        Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room);
    }
}