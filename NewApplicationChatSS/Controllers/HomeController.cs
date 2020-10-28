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
using Microsoft.AspNetCore.Authorization;

namespace NewApplicationChatSS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUserService serv, UserManager<User> manager, IMapper mapper)
        {
            _logger = logger;
            _userService = serv;
            _userManager = manager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<UserDTO> usersDtos = _userService.GetUsersDTO();
            return View(_mapper.Map<List<RegisterViewModel>>(usersDtos));
        }

        [HttpGet]
        public IActionResult Chat()
        {
            return View("Chat");
        }

        [HttpGet]
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
