using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Infrastructure
{
    public class UserValidator : IUserValidator
    {
        public IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserValidator(IUnitOfWork uow, UserManager<User> userManager, IMapper mapper)
        {
            Database = uow;
            _userManager = userManager;
            _mapper = mapper;
        }

        public bool IsUserBlocked(UserDTO userDto)
        {
            throw new NotImplementedException();
        }

        public bool IsUserMuted(string userId, string roomId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserKicked(string userId, string roomId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserInGroup(string userId, string roomId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CommandAccessCheckAsync(UserDTO userDto, IEnumerable<string> allowedRoles, bool checkOnOwner = false, string processingUserName = "")
        {
            foreach(string role in allowedRoles)
            {
                if (await _userManager.IsInRoleAsync(_mapper.Map<User>(userDto), role))
                {
                    return true;
                }
             }
            
            if (checkOnOwner)
            {
                return userDto.UserName == processingUserName;
            }

            return false;  
        }
    }
}