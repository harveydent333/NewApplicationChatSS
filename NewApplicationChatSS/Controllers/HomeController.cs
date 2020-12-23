using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewApplicationChatSS.ViewModels;

namespace NewApplicationChatSS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private const string MainRoomId = "1";

        private readonly ILogger<HomeController> logger;
        private readonly IUserService userService;
        private readonly IRoomService roomService;
        private readonly IMemberService memberService;
        private readonly IMessageService messageService;
        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;
        private readonly IMapper mapper;

        public HomeController(
            IUnitOfWork uow,
            ILogger<HomeController> logger,
            IUserService userService,
            UserManager<User> userManager,
            IMapper mapper,
            IUserValidator userValidator,
            IRoomService roomService,
            IMemberService memberService,
            IMessageService messageService)
        {
            this.logger = logger;
            this.userService = userService;
            this.userManager = userManager;
            this.mapper = mapper;
            this.userValidator = userValidator;
            this.roomService = roomService;
            this.memberService = memberService;
            this.messageService = messageService;
        }

        [HttpGet]
        public IActionResult IndexAsync()
        {
            IEnumerable<UserDTO> usersDtos = userService.GetUsersDTO();
            return View(mapper.Map<List<RegisterViewModel>>(usersDtos));
        }

        [HttpGet("/Chat/{Id?}")]
        public async Task<IActionResult> ChatAsync()
        {
            string roomId = MainRoomId;

            if (await userValidator.IsUserInGroupByNameAsync(User.Identity.Name, roomId))
            {
                ViewBag.UserNotMemberRoom = 1;
                roomId = MainRoomId;
            }
            else
            {
                ViewBag.UserNotMemberRoom = 0;
            }

            if (await userValidator.IsUserKickedByNameAsync(User.Identity.Name, roomId))
            {
                ViewBag.IsKicked = 1;
                roomId = MainRoomId;
            }
            else
            {
                ViewBag.IsKicked = 0;
            }

            ViewBag.Messages = mapper.Map<List<MessageViewModel>>(messageService.GetRoomMessagesDTO(roomId));

            RoomDTO room = roomService.GetRoomDTO(roomId);

            ViewBag.CurrentRoomName = room.RoomName;
            ViewBag.CurrentRoomId = roomId;
            ViewBag.TypeRoom = room.TypeRoom.TypeName;
            ViewBag.UserName = User.Identity.Name;

            string userId = (await userService.GetUserDTObyUserNameAsync(User.Identity.Name)).Id;

            ViewBag.RoomsUser = mapper.Map<List<RoomViewModel>>(memberService.GetRoomsUser(userId));

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
