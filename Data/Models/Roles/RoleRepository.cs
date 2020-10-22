using System.Linq;

namespace AppChatSS.Models.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private ApplicationDbContext context;

        /// <summary>
        /// Возвращает коллекцию записей ролей пользователя
        /// </summary>
        public IQueryable<Role> Roles => context.Roles;

        public RoleRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Метод ищет запись роли в базе данных по id
        /// </summary>
        public Role FindRoleById(int roleId)
        {
            return context.Roles.FirstOrDefault(r => r.Id == roleId);
        }
    }
}