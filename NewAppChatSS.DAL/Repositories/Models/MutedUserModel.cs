using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="MutedUser"/>
    /// </summary>
    public class MutedUserModel : BaseModel<MutedUser, int, ApplicationDbContext>
    {
    }
}
