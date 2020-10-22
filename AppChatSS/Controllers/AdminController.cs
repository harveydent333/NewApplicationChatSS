using System;
using System.Collections.Generic;
using System.Linq;
using AppChatSS.Infrastucture;
using AppChatSS.Models.Messages;
using AppChatSS.Models.Roles;
using AppChatSS.Models.Rooms;
using AppChatSS.Models.SwearingUsers;
using AppChatSS.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppChatSS.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class AdminController : Controller
    {
        const Int32 ROLE_REGULAR_USER = 1;
        const Int32 ROLE_MODERATOR = 2;
        const Int32 INDEFINITE_BLOCKING = 100;

        private static IUserRepository userRepository;
        private static IRoleRepository roleRepository;
        private static IRoomRepository roomRepository;
        private static ISwearingUserRepository swearingUserRepository;
        private static IMessageRepository messageRepository;

        public AdminController(IUserRepository userRep, IRoleRepository roleRep, IRoomRepository roomRep, IMessageRepository messageRep, ISwearingUserRepository swearingUserRep)
        {
            userRepository = userRep;
            roleRepository = roleRep;
            roomRepository = roomRep;
            messageRepository = messageRep;
            swearingUserRepository = swearingUserRep;
        }

        /// <summary>
        /// Метод возвращает административную формы
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View("AdminPage", userRepository.Users.Include(u => u.Role));
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult SetRegularUserRole(Int32 id)
        {
            User user = userRepository.FindUserById(id);
            user.RoleId = ROLE_REGULAR_USER;
            userRepository.EditUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult SetModerationRole(Int32 id)
        {
            User user = userRepository.FindUserById(id);
            user.RoleId = ROLE_MODERATOR;
            userRepository.EditUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult BlockUser(Int32 id)
        {
            User user = userRepository.FindUserById(id);
            user.Loked = true;
            user.DateUnblock = DateTime.Now.AddYears(INDEFINITE_BLOCKING);
            userRepository.EditUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult UnblockUser(Int32 id)
        {
            User user = userRepository.FindUserById(id);
            user.Loked = false;
            userRepository.EditUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RoomStatistics()
        {
            var roomsLinq = from room in roomRepository.Rooms
                            select new RoomStatistic
                            {
                                RoomName = room.RoomName,
                                CountMessage = (from message in messageRepository.Messages
                                                where message.RoomId == room.Id
                                                select message).Count(),
                            };

            roomsLinq = from r in roomsLinq
                        where r.CountMessage > 3
                        select r;

            var rooms = roomRepository.Rooms.Select(r => new RoomStatistic
            {
                RoomName = r.RoomName,
                CountMessage = messageRepository.Messages
                    .Where(m => m.RoomId == r.Id).Count()
            });

            ViewBag.rooms = rooms.Where(r => r.CountMessage > 3);

            return View("RoomStatistics");
        }

        [HttpGet("Admin/SwearingUsers")]
        public IActionResult SwearingUserStatistics()
        {
            ViewBag.SwearingUsers = swearingUserRepository.SwearingUsers
                .Include(u => u.User)
                .OrderByDescending(u => u.CountSwear);

            return View("SwearingUserStatistics");
        }
    }
}
