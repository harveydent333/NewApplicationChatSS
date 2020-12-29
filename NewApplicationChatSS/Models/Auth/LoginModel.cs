using System.ComponentModel.DataAnnotations;

namespace NewApplicationChatSS.Models.Auth
{
    /// <summary>
    /// Данны авторизации пользователя
    /// </summary>
    public sealed class LoginModel
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
