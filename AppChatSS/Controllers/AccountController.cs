using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppChatSS.Infrastucture;
using AppChatSS.Models.KickedOuts;
using AppChatSS.Models.Members;
using AppChatSS.Models.MutedUsers;
using AppChatSS.Models.Roles;
using AppChatSS.Models.Users;
using AppChatSS.Validation;
using AppChatSS.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AppChatSS.Controllers
{
    public class AccountController : Controller
    {
        const String MAIN_ROOM_ID = "1";

        private UserValidator userValidator;
        private IUserRepository userRepository;
        private IRoleRepository roleRepository;
        private IMemberRepository memberRepository;
        private IKickedOutsRepository kickedOutsRepository;
        private IMutedUserRepository mutedUserRepository;

        public AccountController(IUserRepository userRep, IRoleRepository roleRep, IMemberRepository memberRep, IKickedOutsRepository kickedOutsRep, IMutedUserRepository mutedUserRep)
        {
            userRepository = userRep;
            roleRepository = roleRep;
            memberRepository = memberRep;
            kickedOutsRepository = kickedOutsRep;
            mutedUserRepository = mutedUserRep;
            userValidator = new UserValidator(userRep, memberRep, kickedOutsRep, mutedUserRep);
        }

        /// <summary>
        /// Метод возвращает форму авторизации
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        /// <summary>
        /// Метод возвращает форму регистрации
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        /// <summary>
        /// Метод проверяет данные пользователя при аудентификации
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (UserValidator.PresenceCheckUser(loginModel))
                {
                    await Authenticate(userRepository.FindUserByLogin(loginModel.Login));

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректный логин или пароль");
            }

            return View(loginModel);
        }

        /// <summary>
        /// Метод добавляет нового пользователя при регистрации
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                if (UserValidator.UniquenessCheckUser(registerModel.Login))
                {
                    userRepository.AddUser(registerModel);
                    Int32 userId = userRepository.FindUserByLogin(registerModel.Login).Id;
                    memberRepository.AddMember(userId, MAIN_ROOM_ID);

                    await Authenticate(userRepository.FindUserByLogin(registerModel.Login));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с данным логином уже зарегистрирован.");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Метод аудентифицирует пользователя в сети, который ввел логин и пароль 
        /// </summary>
        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roleRepository.FindRoleById(user.RoleId).RoleName)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        /// <summary>
        /// Удаляет аудентифицированные куки пользователя и перенаправляет на форму авторизации
        /// </summary>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }
    }
}
