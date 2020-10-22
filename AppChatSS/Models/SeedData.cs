using AppChatSS.Infrastucture;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using AppChatSS.Models.Roles;
using AppChatSS.Models.Users;
using System.Linq;
using AppChatSS.Models.Rooms;
using AppChatSS.Models.TypeRooms;
using AppChatSS.Models.Members;

namespace AppChatSS.Models
{
    public static class SeedData
    {
        /// <summary>
        /// Метод загружает первые данные в базу данных
        /// </summary>
        public static void InitFirstData(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role
                    {
                        Id = 1,
                        RoleName = "RegularUser"
                    },
                    new Role
                    {
                        Id = 2,
                        RoleName = "Moderator"
                    },
                    new Role
                    {
                        Id = 3,
                        RoleName = "Administrator"
                    }
                );
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.Add(
                    new User
                    {
                        Login = "admin",
                        Email = "admin@mail.ru",
                        Password = HashPassword.GetHashPassword("admin123456"),
                        PasswordConfirm = HashPassword.GetHashPassword("admin123456"),
                        RoleId = 3
                    }
                );
                context.SaveChanges();
            }

            if (!context.TypesRoom.Any())
            {
                context.TypesRoom.AddRange(
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
                    }
                );
                context.SaveChanges();
            }

            if (!context.Rooms.Any())
            {
                context.Rooms.Add(
                    new Room
                    {
                        Id = "1",
                        RoomName = "Главная комната",
                        OwnerId = 1,
                        TypeId = 1,
                    }
                );
                context.SaveChanges();
            }
            if (!context.Members.Any())
            {
                context.Members.Add(
                    new Member
                    {
                        UserId = 1,
                        RoomId = "1"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
