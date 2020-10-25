using Microsoft.AspNetCore.Mvc;
using System;
using Data.Models.Roles;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Users
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public Int32 Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required]
        [Display(Name = "Логин")]
        [RegularExpression(@"[a-zA-Z0-9]+", ErrorMessage = "Поле \"Логин\" может содержать только символы a-z, A-Z, 0-9")]
        [StringLength(35, MinimumLength = 4, ErrorMessage = "Не соотсвествует требованию размера логина, длина должна быть от 4 до 35 символов")]
        public String Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "Не соотсвествует требованию размера пароля, длина должна быть от 6 до  символов")]
        public String Password { get; set; }

        [Required]
        [Display(Name = "Подтвердить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [StringLength(40, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public String PasswordConfirm { get; set; }

        [Required]
        [Display(Name = "E-Mail")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Некорректный адрес электронной почты")]
        [StringLength(60, ErrorMessage = "Не соотсвествует требованию размера E-mail, длина должна быть не больше 60 символов")]
        public String Email { get; set; }

        [Required]
        public Boolean Loked { get; set; } = false;

        /// <summary>
        /// Дата окончания блокировки
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DateUnblock { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        [Required]
        public Role Role { get; set; }

        /// <summary>
        /// Id роли пользователя
        /// </summary>
        [Required]
        public Int32 RoleId { get; set; }
    }
}
