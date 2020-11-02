using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces
{
    public interface IMessageHandler
    {
        Task<string> SaveMessageIntoDatabase(User user, string textMessage, Room room);
    }
}