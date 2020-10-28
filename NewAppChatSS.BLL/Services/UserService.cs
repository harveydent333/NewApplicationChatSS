using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Services
{
    public class UserService : IUserService
    {
        public IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork uow, UserManager<User> userManager, IMapper mapper)
        {
            Database = uow;
            _userManager = userManager;
            _mapper = mapper;
        }

        public IEnumerable<UserDTO> GetUsersDTO()
        {
            return _mapper.Map<IEnumerable<UserDTO>>(Database.Users.GetAll());
        }

        public UserDTO GetUserDTO(string id)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterUser(UserDTO userDTO)
        {
            UserValidator userValidator = new UserValidator(Database);

            if (!userValidator.UniquenessCheckUserLogin(userDTO.Login))
            {
                throw new ValidationException("Пользователь с данным логином уже зарегистрирован.", "");
            }

            if (!userValidator.UniquenessCheckUserEMail(userDTO.Email))
            {
                throw new ValidationException("Данный E-mail адрес уже зарегистрирован.", "");
            }

            await Database.Users.Create(_mapper.Map<User>(userDTO));

            await AssignRoleForNewUser(userDTO.Email);
        }

        public async Task AssignRoleForNewUser(string userEmail)
        {
            User user = await _userManager.FindByEmailAsync(userEmail);
            await _userManager.AddToRolesAsync(user, new string[] { "RegularUser" });
        }
    }
}