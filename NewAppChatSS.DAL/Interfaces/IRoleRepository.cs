using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IRoleRepository
    {
        string FindRoleIdByName(string roleName);
        string FindRoleNameById(string roleId);
    }
}