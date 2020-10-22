using AppChatSS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.Users
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }

        User FindUserById(Int32 userId);

        User FindUserByLogin(String userLogin);

        void AddUser(RegisterModel registerUser);

        void EditUser(User user);

        void Save();
    }
}
