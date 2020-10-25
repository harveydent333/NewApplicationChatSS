using System.Linq;

namespace Data.Models.TypeRooms
{
    public interface ITypeRoomRepository
    {
        IQueryable<TypeRoom> Types { get; }
    }
}
