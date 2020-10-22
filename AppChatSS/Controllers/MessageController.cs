using System;
using System.Collections.Generic;
using System.Linq;
using AppChatSS.Models.Messages;
using AppChatSS.Models.Rooms;
using AppChatSS.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppChatSS.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private IMessageRepository messageRepository;
        private IRoomRepository roomRepository;
        private IUserRepository userRepository;

        public MessageController(IMessageRepository messageRep, IRoomRepository roomRep, IUserRepository userRep)
        {
            messageRepository = messageRep;
            roomRepository = roomRep;
            userRepository = userRep;
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь и если нет, то удаляет сообщение из базы данных
        /// Также проверяет если данное сообщение было последним в комнате, назначает последним, предыдущие до него
        /// </summary>
        [HttpGet("Message/Delete/{roomId}/{messageId}")]
        public IActionResult Delete(String roomId, String messageId)
        {
            User user = userRepository.FindUserById(Int32.Parse(User.Identity.Name));

            if (user.Loked)
            {
                return RedirectToAction("Chat", "Home", new { id = roomId });
            }

            List<String> listLastMessagesIdRoom = roomRepository.Rooms
                .Select(r => r.LastMessageId)
                .ToList();

            if (listLastMessagesIdRoom.Contains(messageId))
            {
                Room processedRoom = roomRepository.Rooms
                    .Where(r => r.LastMessageId == messageId)
                    .FirstOrDefault();

                messageRepository.DeleteMessage(messageId);
                messageRepository.Save();

                Message proccessedMessage = messageRepository.Messages
                    .Where(m => m.RoomId == roomId)
                    .OrderByDescending(m => m.DatePublication)
                    .FirstOrDefault();

                processedRoom.LastMessageId = proccessedMessage?.Id;
                roomRepository.EditRoom(processedRoom);
            }
            else
            {
                messageRepository.DeleteMessage(messageId);
                messageRepository.Save();
            }

            return RedirectToAction("Chat", "Home", new { id = roomId });
        }
    }
}
