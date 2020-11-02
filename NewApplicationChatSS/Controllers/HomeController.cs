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
using Microsoft.AspNetCore.Authorization;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.BLL.Services;

namespace NewApplicationChatSS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        const string MAIN_ROOM_ID = "1";

        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly IMemberService _memberService;
        private readonly IMessageService _messageService;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidator _userValidator;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUserService serv, UserManager<User> manager, IMapper mapper, IUserValidator userValidator, IRoomService roomService, IMemberService memberService, IMessageService messageService)
        {
            _logger = logger;
            _userService = serv;
            _userManager = manager;
            _mapper = mapper;
            _userValidator = userValidator;
            _roomService = roomService;
            _memberService = memberService;
            _messageService = messageService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<UserDTO> usersDtos = _userService.GetUsersDTO();
            return View(_mapper.Map<List<RegisterViewModel>>(usersDtos));
        }

        [HttpGet("/Chat/{Id?}")]
        public async Task<IActionResult> ChatAsync()
        {
            string roomId = MAIN_ROOM_ID;

            if (await _userValidator.IsUserInGroupByNameAsync(User.Identity.Name, roomId))
            {
                ViewBag.UserNotMemberRoom = 1;
                roomId = MAIN_ROOM_ID;
            }
            else
                ViewBag.UserNotMemberRoom = 0;

            if (await _userValidator.IsUserKickedByNameAsync(User.Identity.Name, roomId))
            {
                ViewBag.IsKicked = 1;
                roomId = MAIN_ROOM_ID;
            }
            else
                ViewBag.IsKicked = 0;

            ViewBag.Messages = _mapper.Map<List<MessageViewModel>>(_messageService.GetRoomMessagesDTO(roomId));

            RoomDTO room = _roomService.GetRoomDTO(roomId);

            ViewBag.CurrentRoomName = room.RoomName;
            ViewBag.CurrentRoomId = roomId;
            ViewBag.TypeRoom = room.TypeRoom.TypeName;
            ViewBag.UserName = User.Identity.Name;

            string userId = (await _userService.GetUserDTObyUserNameAsync(User.Identity.Name)).Id;

            ViewBag.RoomsUser = _mapper.Map<List<RoomViewModel>>(_memberService.GetRoomsUser(userId));

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
