using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewApplicationChatSS.ViewModels;

namespace NewApplicationChatSS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginUserModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //ClaimsIdentity id2 = _userService.AuthenticateUser(new UserDTO
                    //{
                    //    Login = loginUserModel.Login,
                    //    PasswordHash = HashPassword.GetHashPassword(loginUserModel.PasswordHash),
                    //});
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimsIdentity.DefaultNameClaimType, "fba6ee9c-e5e1-42a1-be25-5754d92717cd"),
                    //    new Claim(ClaimsIdentity.DefaultRoleClaimType, "RegularUser")
                    //};
                    //ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                    var x = User.Identity;
                    await Authenticate();
                    return RedirectToAction("Index", "Home");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);

                    return View(loginUserModel);
                }
            }
            return View(loginUserModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerUserModel)
        {
            if (ModelState.IsValid)
            {
                try
                {                    
                    await _userService.RegisterUser(new UserDTO
                    {
                        Login = registerUserModel.Login,
                        Email = registerUserModel.Email,
                        PasswordHash = HashPassword.GetHashPassword(registerUserModel.PasswordHash),
                        Loked = registerUserModel.Loked
                    });

                    return RedirectToAction("Index","Home");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);

                    return View(registerUserModel);
                }
            }
            return View(registerUserModel);
        }

        //TEST
        private async Task Authenticate()
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "fba6ee9c-e5e1-42a1-be25-5754d92717cd"),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, "RegularUser")
                    };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
