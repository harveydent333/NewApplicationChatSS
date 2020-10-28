using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewAppChatSS.BLL.Infrastructure
{
    public class UserValidator
    {
        public IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;

        public UserValidator(IUnitOfWork uow, UserManager<User> userManager)
        {
            Database = uow;
            _userManager = userManager;
        }
    }
}