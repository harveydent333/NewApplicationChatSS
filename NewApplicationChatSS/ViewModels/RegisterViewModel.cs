using System.ComponentModel.DataAnnotations;

namespace NewApplicationChatSS.ViewModels
{
    /// <summary>
    /// Регистрационные данные пользователя
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"Имя пользователя\" обязательно к заполнению.")]
        [Display(Name = "Имя пользователя")]
        [RegularExpression(@"[a-zA-Z0-9]+", ErrorMessage = "Поле \"Имя пользователя\" может содержать только символы a-z, A-Z, 0-9")]
        [StringLength(35, MinimumLength = 4, ErrorMessage = "Не соотсвествует требованию размера имени, длина должна быть от 4 до 35 символов")]
        public string UserName { get; set; }

        /// <summary>
        /// E-mail адрес пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"E-Mail\" обязательно к заполнению.")]
        [Display(Name = "E-Mail")]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        [StringLength(60, ErrorMessage = "Не соотсвествует требованию размера E-mail, длина должна быть не больше 60 символов")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"Пароль\" обязательно к заполнению.")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Не соотсвествует требованию размера пароля, длина должна быть от 6 до  символов")]
        public string Password { get; set; }

        /// <summary>
        /// Подтвержденный пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"Подтвердить пароль\" обязательно к заполнению.")]
        [Display(Name = "Подтвердить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// Флаг заблокирован ли пользователь
        /// </summary>
        [Required]
        public bool IsLocked { get; set; } = false;
    }
}
