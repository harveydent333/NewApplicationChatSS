using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMemberRepository : IBaseRepository<Member, int, ApplicationDbContext, MemberModel>
    {
    }
}
