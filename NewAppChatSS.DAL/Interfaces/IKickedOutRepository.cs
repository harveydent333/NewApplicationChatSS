using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IKickedOutRepository : IBaseRepository<KickedOut, int, ApplicationDbContext, KickedOutModel>
    {
    }
}