using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _mapper = mapper;
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
                    await _userService.AuthenticateUserAsync(_mapper.Map<UserDTO>(loginUserModel));
                    return RedirectToAction("Index", "Home");

                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    return View(loginUserModel);
                } 
            }
             return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerUserModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.RegisterUserAsync(_mapper.Map<UserDTO>(registerUserModel));
                    await _signInManager.PasswordSignInAsync(registerUserModel.Email, registerUserModel.Password, true, false);

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

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
