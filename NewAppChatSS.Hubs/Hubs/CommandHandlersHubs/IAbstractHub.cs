﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    public interface IAbstractHub
    {
        Task SearchCommandAsync(User currentUser, Room currentRoom, string command, IHubCallerClients clients);
    }
}
