using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.TypeRooms
{
    public class TypeRoomRepository : ITypeRoomRepository
    {
        private ApplicationDbContext typeRoomContext;

        public TypeRoomRepository(ApplicationDbContext context)
        {
            typeRoomContext = context;
        }

        public IQueryable<TypeRoom> Types => typeRoomContext.TypesRoom;
    }
}
