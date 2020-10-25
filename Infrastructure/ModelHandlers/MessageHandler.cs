using Data.Models.Dictionary_Bad_Words;
using Data.Models.Messages;
using Data.Models.Rooms;
using Data.Models.SwearingUsers;
using Data.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Infrastructure
{
    /// <summary>
    /// Обработчик сообщений в чате
    /// </summary>
    public class MessageHandler
    {
        private static IMessageRepository messageRepository;
        private static IRoomRepository roomRepository;
        private static ISwearingUserRepository swearingUserRepository;
        private static IDictionaryBadWordsRepository dictionaryBadWordsRepository;

        public MessageHandler(IMessageRepository messageRep, IRoomRepository roomRep, ISwearingUserRepository swearingUserRep, IDictionaryBadWordsRepository dictionaryBadWordsRep)
        {
            messageRepository = messageRep;
            roomRepository = roomRep;
            swearingUserRepository = swearingUserRep;
            dictionaryBadWordsRepository = dictionaryBadWordsRep;
        }

        /// <summary>
        /// Метод сохраняет в базу данных информацию о сообщении написаным в пользователем в чат комнаты
        /// </summary>
        public String SaveMessageIntoDatabase(User user, String textMessage, Room room)
        {
            String messageId = Guid.NewGuid().ToString();
            Message message = new Message
            {
                Id = messageId,
                ContentMessage = textMessage,
                DatePublication = DateTime.Now,
                UserId = user.Id,
                RoomId = room.Id
            };

            messageRepository.AddMessage(message);
            AddInfoAboutLastMessage(messageId, room.Id);

            if (ContainsSwearWords(textMessage))
            {
                HandleSwearingUser(user.Id);
            }

            return JsonSerializer.Serialize<object>(new
            {
                userLogin = user.Login,
                messageId = message.Id,
                messageContent = message.ContentMessage,
                datePublication = DateTime.Now,
                roomId = room.Id
            });
        }

        /// <summary>
        /// Метод добавляет информацию в базу данных о последем сообщении в комнате
        /// </summary>
        public static void AddInfoAboutLastMessage(String messageId, String roomId)
        {
            Room room = roomRepository.FindRoomById(roomId);
            room.LastMessageId = messageId;
            roomRepository.EditRoom(room);
        }

        /// <summary>
        /// Метод добавляет информацию о ругающемся пользователе в базу данных
        /// </summary>
        private void HandleSwearingUser(Int32? userId)
        {
            SwearingUser swearingUser = swearingUserRepository.FindSwearingUserByUserId(userId);
            if (swearingUser != null)
            {
                swearingUser.CountSwear++;
                swearingUserRepository.EditSwearingUser(swearingUser);
            }
            else
            {
                swearingUser = new SwearingUser
                {
                    UserId = userId,
                    CountSwear = 1,
                };
                swearingUserRepository.AddSwearingUser(swearingUser);
            }
        }

        /// <summary>
        /// Метод проверяет содержатся ли ругательские слова в тексте сообщения
        /// </summary>
        public static Boolean ContainsSwearWords(String textMessage)
        {
            List<String> wordsList = textMessage.Split(' ').ToList();
            List<String> dictionaryBadWords = dictionaryBadWordsRepository
                .DictionaryBadWords.Select(w => w.Word).ToList(); 

            foreach(String word in wordsList)
            {
                if (dictionaryBadWords.Contains(word))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
