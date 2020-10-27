using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewAppChatSS.BLL.Infrastructure
{
    public class UserValidator
    {
        public IUnitOfWork Database { get; set; }

        public UserValidator(IUnitOfWork uow)
        {
            Database = uow;
        }

        /// <summary>
        /// Метод проверяет на уникальность значение логина
        /// </summary>
        /// <returns>Возвращает True если логин уникальный</returns>
        public bool UniquenessCheckUserLogin (string userLogin)
        {
            return Database.Users.FindByLogin(userLogin) == null;
        }

        /// <summary>
        /// Метод проверяет на уникальность значение логина
        /// </summary>
        /// <returns>Возвращает True если логин уникальный</returns>
        public bool UniquenessCheckUserEMail(string userEmail)
        {
            return Database.Users.FindByEmail(userEmail) == null;
        }
    }
}