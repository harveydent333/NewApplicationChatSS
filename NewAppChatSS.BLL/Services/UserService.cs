using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Services
{
    public sealed class UserService : IUserService
    {
        const string MAIN_ROOM_ID = "1";

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

        public async Task<UserDTO> GetUserDTObyIdAsync(string id)
        {
            return _mapper.Map<UserDTO>(await _userManager.FindByIdAsync(id));
        }

        public async Task<UserDTO> GetUserDTObyUserNameAsync(string userName)
        {
            return _mapper.Map<UserDTO>(await _userManager.FindByNameAsync(userName));
        }

        public async Task<UserDTO> GetUserDTObyEmailAsync(string email)
        {
            return _mapper.Map<UserDTO>(await _userManager.FindByEmailAsync(email));
        }

        public async Task RegisterUserAsync(UserDTO userDTO)
        {
            if (await _userManager.FindByEmailAsync(userDTO.Email) != null)
            {
                throw new ValidationException("Данный E-mail адрес уже зарегистрирован.", "");
            }

            if (await _userManager.FindByNameAsync(userDTO.UserName) != null)
            {
                throw new ValidationException("Пользователь с таким именем уже зарегистрирован.", "");
            }

            User user = _mapper.Map<User>(userDTO);
            user.Id = Guid.NewGuid().ToString();

            await _userManager.CreateAsync(user, userDTO.Password);

            await AssignRoleForNewUserAsync(user);
            await AddingUserInMainRoom(user.Id);
        }

        public async Task AssignRoleForNewUserAsync(User user)
        {
            await _userManager.AddToRolesAsync(user, new string[] { "RegularUser" });
        }

        public async Task AddingUserInMainRoom(string userId)
        {
            await Database.Members.AddMemberAsync(userId, MAIN_ROOM_ID);
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