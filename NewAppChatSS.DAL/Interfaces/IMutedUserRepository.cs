using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IMutedUserRepository : IBaseRepository<MutedUser, int, ApplicationDbContext, MutedUserModel>
    {
    }
}