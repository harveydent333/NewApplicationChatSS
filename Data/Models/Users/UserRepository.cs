//using AppChatSS.Infrastucture;
//using AppChatSS.ViewModels;
using AppChatSS.ViewModels;
using System;
using System.Linq;

namespace AppChatSS.Models.Users
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext userContext;

        public IQueryable<User> Users => userContext.Users;

        public UserRepository(ApplicationDbContext context)
        {
            userContext = context;
        }

        /// <summary>
        /// Метод ищет запись пользователя в базе данных по его id
        /// </summary>
        public User FindUserById(int userId)
        {
            return userContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        /// <summary>
        /// Метод ищет запись пользователя в базе данных по его login 
        /// </summary>
        public User FindUserByLogin(String userLogin)
        {
            return userContext.Users.FirstOrDefault(u => u.Login == userLogin);
        }

        /// <summary>
        /// Метод добавляет запись о пользователе в базу данных
        /// </summary>
        public void AddUser(RegisterModel registerUser)
        {
            userContext.Users.Add(new User
            {
                Login = registerUser.Login,
                Email = registerUser.Email,
             //   Password = HashPassword.GetHashPassword(registerUser.Password),
             //   PasswordConfirm = HashPassword.GetHashPassword(registerUser.PasswordConfirm),
                RoleId = registerUser.RoleId
            });
            Save();
        }

        /// <summary>
        /// Метод изменяет запись о пользователе в базе данных
        /// </summary>
        public void EditUser(User user)
        {
            userContext.Users.Update(user);
            Save();
        }

        /// <summary>
        /// Метод сохраняет состояние записей в базе данных
        /// </summary>
        public void Save()
        {
            userContext.SaveChanges();
        }
    }
}
