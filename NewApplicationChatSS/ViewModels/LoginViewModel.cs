using System;
using System.ComponentModel.DataAnnotations;

namespace NewApplicationChatSS.ViewModels
{
    /// <summary>
    /// Данны авторизации пользователя
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [Display(Name = "Запомнить?")]
        public Boolean RememberMe { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указан Пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public String PasswordHash { get; set; }
    }
}
