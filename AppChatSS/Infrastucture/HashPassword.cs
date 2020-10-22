using System;
using System.Security.Cryptography;
using System.Text;

namespace AppChatSS.Infrastucture
{
    public class HashPassword
    {
        /// <summary>
        /// Метод хеширует строку пароля в md5
        /// </summary>
        public static String GetHashPassword(String password)
        {
            var md5 = MD5.Create();

            return Convert.ToBase64String(
                md5.ComputeHash(Encoding.UTF8.GetBytes(password)
            ));
        }
    }
}
