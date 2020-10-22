using AppChatSS.Hubs;
using AppChatSS.Infrastucture;
using AppChatSS.Models;
using AppChatSS.Models.Roles;
using AppChatSS.Models.Users;
using AppChatSS.Models.Messages;
using AppChatSS.Models.Rooms;
using AppChatSS.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using AppChatSS.Models.Members;
using AppChatSS.Models.MutedUsers;
using AppChatSS.Models.Dictionary_Bad_Words;
using AppChatSS.Models.KickedOuts;
using AppChatSS.Models.Dictionary_Bad_Words;
using AppChatSS.Models.SwearingUsers;

namespace AppChatSS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["Data:TestChatSS:ConnectionString"]));
            
            services.AddControllersWithViews();

            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IMemberRepository, MemberRepository>();
            services.AddTransient<IMutedUserRepository, MutedUserRepository>();
            services.AddTransient<IKickedOutsRepository, KickedOutsRepository>();
            services.AddTransient<IDictionaryBadWordsRepository, DictionaryBadWordsRepository>();
            services.AddTransient<ISwearingUserRepository, SwearingUserRepository>();
            services.AddTransient<ChatHub>();
            services.AddTransient<UserValidator>();
            services.AddTransient<UserCommandHandlerHub>();
            services.AddTransient<RoomCommandHandlerHub>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/");
                });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/Chat/chathub");
            });

            SeedData.InitFirstData(app);
        }
    } 
}
