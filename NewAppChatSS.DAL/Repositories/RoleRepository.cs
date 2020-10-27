using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewAppChatSS.DAL.Repositories
{
    class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _dbRoleContext;

        public RoleRepository(ApplicationDbContext context)
        {
            _dbRoleContext = context;
        }

        public string FindRoleIdByName(string roleName)
        {
            return _dbRoleContext.Roles.FirstOrDefault(r => r.Name == roleName)?.Id;
        }

        public string FindRoleNameById(string roleId)
        {
            return _dbRoleContext.Roles.FirstOrDefault(r => r.Id == roleId)?.Name;
        }
    }
}
