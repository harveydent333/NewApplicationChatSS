﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<User> manager)
        {
            _context = context;
            _userManager = manager;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public async Task<User> FindById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email); 
        }

        public async Task CreateAsync(User item)
        {
            item.Id = Guid.NewGuid().ToString();
            await _userManager.CreateAsync(item, item.PasswordHash);
        }

        public async Task UpdateAsync(User item)
        {
            await _userManager.UpdateAsync(item);
        }

        public async Task DeleteAsync(User item)
        {
            await _userManager.DeleteAsync(item);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
