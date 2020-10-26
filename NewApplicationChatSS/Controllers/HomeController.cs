using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewAppChatSS.DAL.Entities;
using NewApplicationChatSS.Models;
using NewAppChatSS.BLL.DTO;
using AutoMapper;
using NewApplicationChatSS.ViewModels;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.Interfaces;

namespace NewApplicationChatSS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;
        private UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, IUserService serv, UserManager<User> manager)
        {
            _logger = logger;
            _userService = serv;
            _userManager = manager;
        }

        public IActionResult Index()
        {
            IEnumerable<UserDTO> usersDtos = _userService.GetUsersDTO();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, RegisterViewModel>()).CreateMapper();
            var users = mapper.Map<IEnumerable<UserDTO>, List<RegisterViewModel>>(usersDtos);
            return View(users);
            //return View("Chat");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
