using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces
{
    public interface IMessageHandler
    {
        string SaveMessageIntoDatabase(User user, string textMessage, Room room);
    }
}