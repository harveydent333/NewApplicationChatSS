using System;
using System.ComponentModel.DataAnnotations;

namespace AppChatSS.ViewModels
{
    /// <summary>
    /// Регистрационные данные пользователя
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"Логин\" обязательно к заполнению.")]
        [Display(Name = "Логин")]
        [RegularExpression(@"[a-zA-Z0-9]+", ErrorMessage = "Поле \"Логин\" может содержать только символы a-z, A-Z, 0-9")]
        [StringLength(35, MinimumLength = 4, ErrorMessage = "Не соотсвествует требованию размера логина, длина должна быть от 4 до 35 символов")]
        public String Login { get; set; }

        /// <summary>
        /// E-mail адрес пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"E-Mail\" обязательно к заполнению.")]
        [Display(Name = "E-Mail")]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        [StringLength(60, ErrorMessage = "Не соотсвествует требованию размера E-mail, длина должна быть не больше 60 символов")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"Пароль\" обязательно к заполнению.")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Не соотсвествует требованию размера пароля, длина должна быть от 6 до  символов")]
        public String Password { get; set; }

        /// <summary>
        /// Подтвержденный пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Поле \"Подтвердить пароль\" обязательно к заполнению.")]
        [Display(Name = "Подтвердить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public String PasswordConfirm { get; set; }

        /// <summary>
        /// Id роли пользователя
        /// </summary>
        [Required]
        public Int32 RoleId { get; set; }

        /// <summary>
        /// Флаг заблокирован ли пользователь
        /// </summary>
        [Required]
        public Boolean Loked { get; set; } = false;
    }
}
