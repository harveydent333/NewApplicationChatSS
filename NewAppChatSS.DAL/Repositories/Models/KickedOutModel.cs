﻿using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Repositories.Models
{
    /// <summary>
    /// Модель для фильтрации <see cref="KickedOut"/>
    /// </summary>
    public class KickedOutModel : BaseModel<KickedOut, int, ApplicationDbContext>
    {
    }
}