using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Models.Messages
{
    public class MessageRepository : IMessageRepository
    {
        private ApplicationDbContext messageContext;

        public MessageRepository(ApplicationDbContext context)
        {
            messageContext = context;
        }

        /// <summary>
        /// Возвращает коллекцию записей сообщений
        /// </summary>
        public IQueryable<Message> Messages => messageContext.Messages;

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public void AddMessage(Message message)
        {
            messageContext.Add(message);
            Save();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public void DeleteMessage(String Id)
        {
            messageContext
                .Remove(messageContext.Messages
                .Where(m => m.Id == Id)
                .FirstOrDefault());
        }
        
        /// <summary>
        /// Метод ищет сообщения в комнате
        /// </summary>
        public List<Message> FindMessagesByRoomId(string roomId)
        {
            return messageContext.Messages
                .Where(m => m.RoomId == roomId)
                .ToList();
        }

        /// <summary>
        /// Метод сохраняет изменения состояния в базе данных
        /// </summary>
        public void Save()
        {
            messageContext.SaveChanges();
        }
    }
}