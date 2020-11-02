using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewAppChatSS.DAL.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Метод возвращает все сообщения
        /// </summary>
        public IEnumerable<Message> GetAll()
        {
            return _context.Messages.ToList();
        }

        /// <summary>
        /// Добавляет запись в таблицу базы данных
        /// </summary>
        public void AddMessage(Message item)
        {
            _context.Messages.Add(item);
            Save();
        }

        /// <summary>
        /// Метод удаляет запись из таблицы базы данных
        /// </summary>
        public void DeleteMessage(string id)
        {
            _context.Messages.Remove(
                _context.Messages.FirstOrDefault(m => m.Id == id));
        }

        /// <summary>
        /// Метод ищет сообщения в комнате
        /// </summary>
        public IEnumerable<Message> FindMessagesByRoomId(string roomId)
        {
            return _context.Messages
                .Where(m => m.RoomId == roomId)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Message> GetRoomMessages(string roomId)
        {
            return _context.Messages
                .Include(m => m.User)
                .Include(m => m.Room)
                .Where(m => m.RoomId == roomId)
                .OrderBy(m => m.DatePublication)
                .ToList();
        }
            /// <summary>
            /// Метод сохраняет изменения состояния в базе данных
            /// </summary>
            public void Save()
        {
            _context.SaveChanges();
        }
    }
}