using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewAppChatSS.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string FindRoleIdByName(string roleName)
        {
            return _context.Roles.FirstOrDefault(r => r.Name == roleName)?.Id;
        }

        public string FindRoleNameById(string roleId)
        {
            return _context.Roles.FirstOrDefault(r => r.Id == roleId)?.Name;
        }
    }
}
