using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.Roles
{
    public interface IRoleRepository
    {
        IQueryable<Role> Roles { get; }
        Role FindRoleById(Int32 roleId);
    }
}
