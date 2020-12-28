using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;

        public UserRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public IEnumerable<User> GetAll()
        {
            return context.Users.ToList();
        }

        public async Task<User> FindById(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task CreateAsync(User item)
        {
            item.Id = NewAppChatGuidHelper.GetNewGuid();
            await userManager.CreateAsync(item, item.PasswordHash);
        }

        public async Task UpdateAsync(User item)
        {
            await userManager.UpdateAsync(item);
        }

        public async Task DeleteAsync(User item)
        {
            await userManager.DeleteAsync(item);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
