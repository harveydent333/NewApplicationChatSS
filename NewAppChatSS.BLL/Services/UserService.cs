using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.Constants;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Services
{
    public sealed class UserService : IUserService
    {
        private IUnitOfWork Database { get; set; }

        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly SignInManager<User> signInManager;

        public UserService(IUnitOfWork uow, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            Database = uow;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        public List<UserDTO> GetUsersDTO()
        {
            return mapper.Map<List<UserDTO>>(Database.Users.GetAll());
        }

        public async Task<UserDTO> GetUserDTObyIdAsync(string id)
        {
            return mapper.Map<UserDTO>(await userManager.FindByIdAsync(id));
        }

        public async Task<UserDTO> GetUserDTObyUserNameAsync(string userName)
        {
            return mapper.Map<UserDTO>(await userManager.FindByNameAsync(userName));
        }

        public async Task<UserDTO> GetUserDTObyEmailAsync(string email)
        {
            return mapper.Map<UserDTO>(await userManager.FindByEmailAsync(email));
        }

        public async Task RegisterUserAsync(UserDTO userDTO)
        {
            if (await userManager.FindByEmailAsync(userDTO.Email) != null)
            {
                throw new ValidationException("Данный E-mail адрес уже зарегистрирован.", "");
            }

            if (await userManager.FindByNameAsync(userDTO.UserName) != null)
            {
                throw new ValidationException("Пользователь с таким именем уже зарегистрирован.", "");
            }

            User user = mapper.Map<User>(userDTO);
            user.Id = NewAppChatGuidHelper.GetNewGuid();

            await userManager.CreateAsync(user, userDTO.Password);

            await AssignRoleForNewUserAsync(user);
            await AddingUserInMainRoom(user.Id);
        }

        public async Task AssignRoleForNewUserAsync(User user)
        {
            await userManager.AddToRolesAsync(user, new string[] { "RegularUser" });
        }

        public async Task AddingUserInMainRoom(string userId)
        {
            await Database.Members.AddMemberAsync(userId, GlobalConstants.MainRoomId);
        }

        public async Task AuthenticateUserAsync(UserDTO userDTO)
        {
            User user = await userManager.FindByEmailAsync(userDTO.Email);

            if (user == null)
            {
                throw new ValidationException("Пользователь не найден", "");
            }

            var result = await signInManager.PasswordSignInAsync(user.UserName, userDTO.Password, (bool)userDTO.RememberMe, false);

            if (!result.Succeeded)
            {
                throw new ValidationException("Неправильный логин или пароль", "");
            }
        }
    }
}