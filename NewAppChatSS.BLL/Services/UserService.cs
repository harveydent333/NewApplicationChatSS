using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Services
{
    public class UserService : IUserService
    {
        public IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IEnumerable<UserDTO> GetUsersDTO()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(Database.Users.GetAll());
        }

        public UserDTO GetUserDTO(String id)
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

            await Database.Users.Create(new User
            {
                UserName = userDTO.Email,
                Email = userDTO.Email,
                Login = userDTO.Login,
                Loked = userDTO.Loked,
                RoleId = Database.Roles.FindRoleIdByName("RegularUser"),
                PasswordHash = userDTO.PasswordHash,
            });
        }

        public ClaimsIdentity AuthenticateUser(UserDTO userDTO)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            UserDTO user = mapper.Map<User, UserDTO>(Database.Users.FindByLogin(userDTO.Login));

            if (user != null)
            {
                if (user.PasswordHash == userDTO.PasswordHash)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, Database.Roles.FindRoleNameById(user.RoleId))
                    };          

                    return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                }
                else
                {
                    throw new ValidationException("Неправильный логин и (или) пароль", "");
                }
            }
            else
            {
                throw new ValidationException("Неправильный логин и (или) пароль", "");
            }
        }
    }
}