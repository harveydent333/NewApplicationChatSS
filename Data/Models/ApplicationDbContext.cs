using Microsoft.EntityFrameworkCore;
using AppChatSS.Models.Users;
using AppChatSS.Models.Roles;
using AppChatSS.Models.Messages;
using AppChatSS.Models.Rooms;
using AppChatSS.Models.Members;
using AppChatSS.Models.TypeRooms;
using AppChatSS.Models.KickedOuts;
using AppChatSS.Models.MutedUsers;
using AppChatSS.Models.SwearingUsers;
using AppChatSS.Models.Dictionary_Bad_Words;

namespace AppChatSS.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TypeRoom> TypesRoom { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<KickedOut> KickedOuts { get; set; }
        public DbSet<MutedUser> MutedUsers { get; set; }
        public DbSet<DictionaryBadWords> DicrtionaryBadWords { get; set; }
        public DbSet<SwearingUser> SwearingUsers { get; set; }
    }
}
