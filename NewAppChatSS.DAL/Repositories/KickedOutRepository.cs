using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Repositories
{
    public class KickedOutRepository : BaseRepository<KickedOut, int, NewAppChatSSContext, KickedOutModel>, IKickedOutRepository
    {
        public KickedOutRepository(NewAppChatSSContext context)
            : base(context)
        {
        }
    }
}
