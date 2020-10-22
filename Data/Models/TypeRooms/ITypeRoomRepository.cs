using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.TypeRooms
{
    public interface ITypeRoomRepository
    {
        IQueryable<TypeRoom> Types { get; }
    }
}
