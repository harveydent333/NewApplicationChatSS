using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL.Entities;  
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NewAppChatSS.DAL
{
    public class DataInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "_Qq123456";

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

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, IsLocked = false, Login = "admin" };
                IdentityResult result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }
            }
        }
    }
}
