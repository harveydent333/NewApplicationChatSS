using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System;
using AppChatSS.Models.Rooms;
using AppChatSS.Models.Users;
using System.Collections.Generic;
using AppChatSS.Models.Messages;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AppChatSS.Models.Members;
using AppChatSS.Validation;
using AppChatSS.Models.KickedOuts;
using AppChatSS.Models.MutedUsers;

namespace AppChatSS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUserRepository userRepository;
        private IRoomRepository roomRepository;
        private IMessageRepository messageRepository;
        private IMemberRepository memberRepository;
        private IKickedOutsRepository kickedOutsRepository;
        private IMutedUserRepository mutedUserRepository;
        private UserValidator userValidator;

        const String MAIN_ROOM_ID = "1";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRep, IRoomRepository roomRep, IMessageRepository messageRep, IMemberRepository memberRep, IKickedOutsRepository kickedOutsRep, IMutedUserRepository mutedUserRep)
        {
            _logger = logger;
            userRepository = userRep;
            roomRepository = roomRep;
            messageRepository = messageRep;
            memberRepository = memberRep;
            kickedOutsRepository = kickedOutsRep;
            mutedUserRepository = mutedUserRep;
            userValidator = new UserValidator(userRepository, memberRepository, kickedOutsRepository, mutedUserRepository);
        }

        /// <summary>
        /// Метод возвращает главную форму
        /// </summary>
        [AllowAnonymous]
        public IActionResult Index()
        {
            try
            {
                ViewBag.LoginUser = userRepository.FindUserById(Int32.Parse(User.Identity.Name)).Login;
            }
            catch (Exception ex) { }

            return View();
        }

        /// <summary>
        /// Метод принимает Id комнаты в которую переходит пользователь, проверяет состоит ли пользователь в комнате и не исключен ли он из нее
        /// Возвращает форму с чатом
        /// </summary>
        [HttpGet("Chat/{Id?}")]
        public IActionResult Chat(String id = MAIN_ROOM_ID)
        {
            if (!UserValidator.IsUserInGroup(Int32.Parse(User.Identity.Name), id))
            {
                ViewBag.UserNotMemberRoom = 1;
                id = MAIN_ROOM_ID;
            }
            else
                ViewBag.UserNotMemberRoom = 0;

            if (UserValidator.IsUserKicked(Int32.Parse(User.Identity.Name), id))
            {
                ViewBag.IsKicked = 1;
                id = MAIN_ROOM_ID;
            }
            else
                ViewBag.IsKicked = 0;

            ViewBag.Messages = messageRepository.Messages
                .Include(u => u.User)
                .Where(m => m.RoomId == id)
                .OrderBy(m => m.DatePublication)
                .ToList();

            ViewBag.CurrentRoomName = roomRepository.FindRoomById(id).RoomName;

            ViewBag.CurrentRoomId = id;

            ViewBag.TypeRoom = roomRepository.FindRoomById(id).TypeRoom.TypeName;

            ViewBag.LoginUser = userRepository.FindUserById(Int32.Parse(User.Identity.Name)).Login;

            Int32 userId = Int32.Parse(User.Identity.Name);

            List<Room> roomsUser = memberRepository.Members
                .Where(m => m.UserId == userId)
                .Include(r => r.Room)
                .Include(m => m.Room.LastMessage)
                .Select(r => r.Room)
                .ToList();

            return View("Chat", roomsUser);
        }
    }
}
