using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpPost]
        public async Task<IActionResult> Register2(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, Loked = model.Loked, RoleId = model.RoleId, Login = model.Login };
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("Register",model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerUserModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserDTO userDto = new UserDTO
                    {
                        UserName = registerUserModel.Email,
                        Login = registerUserModel.Login,
                        Email = registerUserModel.Email,
                        Password = HashPassword.GetHashPassword(registerUserModel.Password),
                        RoleId = registerUserModel.RoleId,
                        Loked = registerUserModel.Loked
                    };

                    await _userService.RegisterUser(userDto);
                    return RedirectToAction("Index","Home");
                }
                catch (Exception ex)
                {
                    //  ModelState.AddModelError(ex.Property, ex.Message);
                    return View(registerUserModel);
                }
            }
            return View(registerUserModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
