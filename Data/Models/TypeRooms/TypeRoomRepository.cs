using System.Linq;

namespace Data.Models.TypeRooms
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
