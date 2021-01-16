using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;
using NewApplicationChatSS.Models;
using NewApplicationChatSS.Models.Auth;

namespace NewApplicationChatSS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IUserService userService;
        private readonly IRoomService roomService;
        private readonly IMemberService memberService;
        private readonly IMessageService messageService;
        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;
        private readonly IMapper mapper;

        public HomeController(
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
            List<UserDTO> usersDtos = userService.GetUsers();
            return View(mapper.Map<List<RegisterModel>>(usersDtos));
        }

        [HttpGet("/Chat/{roomId?}")]
        public async Task<IActionResult> ChatAsync(string roomId)
        {
            var room = await roomService.GetRoomAsync(roomId);
            var user = await userService.GetUserbyUserNameAsync(User.Identity.Name);

            // TODO: сделать проверку [FromRoute] на roomId и выкидывать 404 в случае не найденой комнаты
            if (roomId == null)
            {
                roomId = GlobalConstants.MainRoomId;
            }

            // TODO: сделать метод универсальным, передавать User user, удалить лишний IsUserInGroupByNameAsync и IsUserInGroupByIdAsync. Сделать общий IsUserInGroupAsync
            if (!await userValidator.IsUserInGroupAsync(User.Identity.Name, roomId))
            {
                ViewBag.UserNotMemberRoom = 1;
                roomId = GlobalConstants.MainRoomId;
            }
            else
            {
                ViewBag.UserNotMemberRoom = 0;
            }

            if (await userValidator.IsUserKickedAsync(user.Id, roomId))
            {
                ViewBag.IsKicked = 1;
                roomId = GlobalConstants.MainRoomId;
            }
            else
            {
                ViewBag.IsKicked = 0;
            }

            ViewBag.Messages = mapper.Map<List<MessageModel>>(await messageService.GetRoomMessagesAsync(roomId));

            room = await roomService.GetRoomAsync(roomId);

            ViewBag.CurrentRoomName = room.RoomName;
            ViewBag.CurrentRoomId = roomId;
            ViewBag.TypeRoom = room.TypeRoom.TypeName;
            ViewBag.UserName = User.Identity.Name;

            ViewBag.RoomsUser = mapper.Map<List<RoomModel>>(await memberService.GetUserRoomsAsync(user.Id));

            return View("Chat");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
