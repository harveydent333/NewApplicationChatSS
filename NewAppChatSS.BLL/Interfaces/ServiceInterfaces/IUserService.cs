using NewAppChatSS.BLL.DTO;
using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserDTO userDTO);

        Task AssignRoleForNewUserAsync(User user);

        IEnumerable<UserDTO> GetUsersDTO();

        UserDTO GetUserDTObyId(string id);

        UserDTO GetUserDTObyEmail(string email);

        UserDTO GetUserDTObyUserName(string userName);

        Task AuthenticateUserAsync(UserDTO userDTO);
    }
}