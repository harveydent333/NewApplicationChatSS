using System;
using System.ComponentModel.DataAnnotations;

namespace NewApplicationChatSS.ViewModels
{
    /// <summary>
    /// Данны авторизации пользователя
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указан Логин")]
        [Display(Name = "Логин")]
        public String Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указан Пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}
