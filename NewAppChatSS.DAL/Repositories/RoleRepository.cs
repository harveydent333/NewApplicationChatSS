using System.Linq;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext context;

        public RoleRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public string FindRoleIdByName(string roleName)
        {
            return context.Roles.FirstOrDefault(r => r.Name == roleName)?.Id;
        }

        public string FindRoleNameById(string roleId)
        {
            return context.Roles.FirstOrDefault(r => r.Id == roleId)?.Name;
        }
    }
}
