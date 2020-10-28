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
        private IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;

        public UserService(IUnitOfWork uow, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            Database = uow;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public IEnumerable<UserDTO> GetUsersDTO()
        {
            return _mapper.Map<IEnumerable<UserDTO>>(Database.Users.GetAll());
        }

        public UserDTO GetUserDTO(string id)
        {
            return _mapper.Map<UserDTO>(_userManager.FindByIdAsync(id));
        }

        public async Task RegisterUserAsync(UserDTO userDTO)
        {
            if (await _userManager.FindByEmailAsync(userDTO.Email) != null)
            {
                throw new ValidationException("Данный E-mail адрес уже зарегистрирован.", "");
            }

            User user = _mapper.Map<User>(userDTO);
            user.Id = Guid.NewGuid().ToString();

            await _userManager.CreateAsync(user, userDTO.Password);

            await AssignRoleForNewUserAsync(userDTO.Email);
        }

        public async Task AssignRoleForNewUserAsync(string userEmail)
        {
            await _userManager.AddToRolesAsync(Database.Users.FindByEmail(userEmail), new string[] { "RegularUser" });
        }

        public async Task AuthenticateUserAsync(UserDTO userDTO)
        {
            User user = await _userManager.FindByEmailAsync(userDTO.Email);
            var result = await _signInManager.PasswordSignInAsync(user.UserName,
                userDTO.Password, (bool)userDTO.RememberMe, false);

            if (!result.Succeeded)
            {
                throw new ValidationException("Неправильный логин или пароль", "");
            }
        }
    }
}