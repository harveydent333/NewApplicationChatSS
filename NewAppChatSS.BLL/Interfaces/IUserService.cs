using NewAppChatSS.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces
{
    public interface IUserService
    {
        Task RegisterUser(UserDTO userDTO);
        Task AssignRoleForNewUser(string userEmail);
        IEnumerable<UserDTO> GetUsersDTO();
        UserDTO GetUserDTO(string id);
        //void AuthenticateUser(UserDTO userDTO);
    }
}