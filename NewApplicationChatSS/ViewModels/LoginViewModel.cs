using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace NewApplicationChatSS.ViewModels
{
    /// <summary>
    /// Данны авторизации пользователя
    /// </summary>
    public sealed class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Не указан Пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
