using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NewApplicationChatSS.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        public MessageController()
        {
    
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь и если нет, то удаляет сообщение из базы данных
        /// Также проверяет если данное сообщение было последним в комнате, назначает последним, предыдущие до него
        /// </summary>
        [HttpGet("Message/Delete/{roomId}/{messageId}")]
        public IActionResult Delete(string roomId, string messageId)
        {
            /*
            UserViewModel user = userRepository.FindUserById(int.Parse(User.Identity.Name));

            if (user.Loked)
            {
                return RedirectToAction("Chat", "Home", new { id = roomId });
            }

            List<string> listLastMessagesIdRoom = roomRepository.Rooms
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
            */
            return RedirectToAction("Chat", "Home", new { id = roomId });
        }
    }
}
