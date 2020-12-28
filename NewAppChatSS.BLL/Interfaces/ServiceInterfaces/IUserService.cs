using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserDTO userDTO);

        Task AssignRoleForNewUserAsync(User user);

        List<UserDTO> GetUsers();

        Task<UserDTO> GetUserbyIdAsync(string id);

        Task<UserDTO> GetUserbyEmailAsync(string email);

        Task<UserDTO> GetUserbyUserNameAsync(string userName);

        Task AuthenticateUserAsync(UserDTO userDTO);
    }
}