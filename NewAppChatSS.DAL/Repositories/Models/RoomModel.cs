﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="Room"/>
    /// </summary>
    public class RoomModel : BaseModel<Room, string, ApplicationDbContext>
    {
        /// <summary>
        /// Нужно ли возвращать владельца комнаты <see cref="Room.Owner"/>
        /// </summary>
        public bool IncludeOwner { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Нужно ли возвращать тип комнаты <see cref="Room.TypeRoom"/>
        /// </summary>
        public bool IncludeTypeRoom { get; set; }

        /// <summary>
        /// Идентификатор типа комнаты
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Нужно ли возвращать последнее сообщение в комнате <see cref="Room.LastMessage"/>
        /// </summary>
        public bool IncludeLastMessage { get; set; }

        /// <summary>
        /// Идентификатор последнего сообщения
        /// </summary>
        public string LastMessage { get; set; }

        public override IQueryable<Room> GetQuarable(ApplicationDbContext context)
        {
            var query = base.GetQuarable(context);

            if (OwnerId != null)
            {
                query = query.Where(q => q.OwnerId == OwnerId);
            }

            query = AddOwner(query, IncludeOwner);

            return query;
        }

        protected IQueryable<Room> AddOwner(IQueryable<Room> query, bool includeOwner)
        {
            if (includeOwner)
            {
                query = query.Include(l => l.Owner);
            }

            return query;
        }

        protected IQueryable<Room> AddTypeRoom(IQueryable<Room> query, bool includeTypeRoom)
        {
            if (includeTypeRoom)
            {
                query = query.Include(l => l.TypeRoom);
            }

            return query;
        }

        protected IQueryable<Room> AddLastMessage(IQueryable<Room> query, bool includeLastMessage)
        {
            if (includeLastMessage)
            {
                query = query.Include(l => l.LastMessage);
            }

            return query;
        }
    }
}