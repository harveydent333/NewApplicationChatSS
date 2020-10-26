using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.DTO;
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
        IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork uow, UserManager<User> userManager)
        {
            Database = uow;
            _userManager = userManager;
        }

        public IEnumerable<UserDTO> GetUsersDTO()
        {
            // применяем автомаппер для проекции одной коллекции на другую
            //var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            //return mapper.Map<IEnumerable<User>, List<UserDTO>>(Database.Users.GetAll());

            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()
                .ForMember("Login", opt => opt.MapFrom(src => src.UserName)));
            var mapper = new Mapper(config);
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(Database.Users.GetAll());
        }

        public UserDTO GetUserDTO(String id)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterUser(UserDTO userDTO)
        {
            User user = new User
            {
                UserName = userDTO.Email,
                Email = userDTO.Email,
                Login = userDTO.Login,
                Loked = userDTO.Loked,
                RoleId = userDTO.RoleId,
                PasswordHash = userDTO.Password,
            };

            //var config = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()
            //    .ForMember("UserName", opt => opt.MapFrom(src => src.Login)));
            //var mapper = new Mapper(config);
            //Database.Users.Create(mapper.Map<UserDTO, User>(userDTO));

            await Database.Users.Create(user);
        }

    }
}
