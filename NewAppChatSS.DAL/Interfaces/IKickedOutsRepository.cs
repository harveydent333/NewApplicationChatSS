﻿using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IKickedOutsRepository
    {
        IEnumerable<KickedOut> GetAll();

        Task AddKickedUserAsync(string userId, string roomId, DateTime dateUnkick);

        Task DeleteKickedUserAsync(string userId, string roomId);

        IEnumerable<KickedOut> GetListKickedRoomForUser(string userId);

        IEnumerable<string> GetListIdsKickedRoomForUser(string userId);

        Task SaveAsync();
    }
}