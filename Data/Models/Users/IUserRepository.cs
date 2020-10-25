using Data.ViewModels;
using System;
using System.Linq;

namespace Data.Models.Users
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
