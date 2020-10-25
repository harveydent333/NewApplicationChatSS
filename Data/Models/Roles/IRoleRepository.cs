using System;
using System.Linq;

namespace Data.Models.Roles
{
    public interface IRoleRepository
    {
        IQueryable<Role> Roles { get; }
        Role FindRoleById(Int32 roleId);
    }
}
