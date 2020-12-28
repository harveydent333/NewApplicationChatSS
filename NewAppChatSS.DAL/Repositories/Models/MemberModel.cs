using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="Member"/>
    /// </summary>
    public class MemberModel : BaseModel<Member, int, ApplicationDbContext>
    {
    }
}