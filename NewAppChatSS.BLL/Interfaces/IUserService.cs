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
        IEnumerable<UserDTO> GetUsersDTO();
        UserDTO GetUserDTO(String id);
        ClaimsIdentity AuthenticateUser(UserDTO userDTO);
    }
}