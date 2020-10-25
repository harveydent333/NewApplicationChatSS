using Microsoft.EntityFrameworkCore;
using Data.Models.Users;
using Data.Models.Roles;
using Data.Models.Messages;
using Data.Models.Rooms;
using Data.Models.Members;
using Data.Models.TypeRooms;
using Data.Models.KickedOuts;
using Data.Models.MutedUsers;
using Data.Models.SwearingUsers;
using Data.Models.Dictionary_Bad_Words;

namespace Data.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TypeRoom> TypesRoom { get; set; }
        public DbSet<KickedOut> KickedOuts { get; set; }
        public DbSet<MutedUser> MutedUsers { get; set; }
        public DbSet<DictionaryBadWords> DicrtionaryBadWords { get; set; }
        public DbSet<SwearingUser> SwearingUsers { get; set; }
    }
}
