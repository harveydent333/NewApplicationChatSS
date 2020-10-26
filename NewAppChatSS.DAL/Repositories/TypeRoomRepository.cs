using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class TypeRoomRepository : IRepository<TypeRoom>
    {
        private ApplicationDbContext dbTypeRoomContext;

        public TypeRoomRepository(ApplicationDbContext context)
        {
            dbTypeRoomContext = context;
        }

        public async Task Create(TypeRoom item)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TypeRoom> Find(Func<TypeRoom, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public TypeRoom Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TypeRoom> GetAll()
        {
            return dbTypeRoomContext.TypesRooms;
        }

        public void Update(TypeRoom item)
        {
            throw new NotImplementedException();
        }
    }
}
