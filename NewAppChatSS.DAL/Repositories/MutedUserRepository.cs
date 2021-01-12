using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class MutedUserRepository : BaseRepository<MutedUser, int, NewAppChatSSContext, MutedUserModel>, IMutedUserRepository
    {
        public MutedUserRepository(NewAppChatSSContext context)
            : base(context)
        {
        }
    }
}