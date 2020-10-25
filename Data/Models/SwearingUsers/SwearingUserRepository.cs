using System;
using System.Linq;

namespace Data.Models.SwearingUsers
{
    public class SwearingUserRepository : ISwearingUserRepository
    {
        private ApplicationDbContext swearingUserContext;

        public SwearingUserRepository(ApplicationDbContext context)
        {
            swearingUserContext = context;
        }

        public IQueryable<SwearingUser> SwearingUsers => swearingUserContext.SwearingUsers;

        public void AddSwearingUser(SwearingUser swearingUse)
        {
            swearingUserContext.Add(swearingUse);
            Save();
        }

        /// <summary>
        /// Метод изменяет запись в таблице базы данных
        /// </summary>
        public void EditSwearingUser(SwearingUser swearingUser)
        {
            swearingUserContext.Update(swearingUser);

            Save();
        }

        public SwearingUser FindSwearingUserByUserId(Int32? userId)
        {
            return swearingUserContext.SwearingUsers
                .Where(s => s.UserId == userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Метод сохраняет состояние записей в базе данных
        /// </summary>
        public void Save()
        {
            swearingUserContext.SaveChanges();
        }
    }
}
