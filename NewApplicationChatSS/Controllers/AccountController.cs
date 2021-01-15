using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Entities;
using NewApplicationChatSS.Models.Auth;

namespace NewApplicationChatSS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AccountController(
            SignInManager<User> signInManager,
            IUserService userService,
            IMapper mapper)
        {
            this.signInManager = signInManager;
            this.userService = userService;
            this.mapper = mapper;
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
        public async Task<IActionResult> LoginAsync(LoginModel loginUserModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await userService.AuthenticateUserAsync(mapper.Map<UserDTO>(loginUserModel));
                    return RedirectToAction("Index", "Home");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(loginUserModel);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterModel registerUserModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await userService.RegisterUserAsync(mapper.Map<UserDTO>(registerUserModel));
                    await signInManager.PasswordSignInAsync(registerUserModel.Email, registerUserModel.Password, true, false);

                    return RedirectToAction("Index", "Home");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);

                    return View(registerUserModel);
                }
            }

            return View(registerUserModel);
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
