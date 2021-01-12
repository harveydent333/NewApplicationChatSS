using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class MemberRepository : BaseRepository<Member, int, NewAppChatSSContext, MemberModel>, IMemberRepository
    {
        public MemberRepository(NewAppChatSSContext context)
            : base(context)
        {
        }
    }
}