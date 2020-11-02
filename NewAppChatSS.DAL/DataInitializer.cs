using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL.Entities;  
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace NewAppChatSS.DAL
{
    public class DataInitializer
    {
        const string MAIN_ROOM_ID = "1";

        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            if (await roleManager.FindByNameAsync("RegularUser") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("RegularUser"));
            }

            if (await roleManager.FindByNameAsync("Moderator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

            if (await roleManager.FindByNameAsync("Administrator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            if (await userManager.FindByNameAsync("admin@gmail.com") == null)
            {
                User admin = new User 
                { 
                    Email = "admin@gmail.com",
                    UserName = "admin",
                    IsLocked = false
                };

                IdentityResult result = await userManager.CreateAsync(admin, "_Qq123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }
            }
            if (!dbContext.TypesRooms.Any())
            {
                dbContext.TypesRooms.AddRange(
                    new TypeRoom
                    {
                        Id = 1,
                        TypeName = "RegularRoom",
                    },
                    new TypeRoom
                    {
                        Id = 2,
                        TypeName = "PrivateRoom",
                    },
                    new TypeRoom
                    {
                        Id = 3,
                        TypeName = "BotRoom",
                    });
                dbContext.SaveChanges();
            };

            if (!dbContext.Rooms.Any())
            {
                dbContext.Rooms.Add(
                    new Room
                    {
                        Id = "1",
                        RoomName = "MainRoom",
                        OwnerId = (await userManager.FindByEmailAsync("admin@gmail.com")).Id,
                        TypeId = 1,
                    });
                dbContext.SaveChanges();
            }

            if (!dbContext.Members.Any())
            {
                dbContext.Members.Add(
                    new Member
                    {
                        UserId = (await userManager.FindByEmailAsync("admin@gmail.com")).Id,
                        RoomId = MAIN_ROOM_ID
                    });
                dbContext.SaveChanges();
            }
        }
    }
}
