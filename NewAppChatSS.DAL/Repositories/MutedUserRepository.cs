using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class MutedUserRepository : BaseRepository<MutedUser, int, ApplicationDbContext, MutedUserModel>, IMutedUserRepository
    {
        public MutedUserRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}