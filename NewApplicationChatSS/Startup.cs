using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.BLL.Services;
using NewAppChatSS.DAL;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories;
using NewAppChatSS.Hubs.Hubs;
using NewAppChatSS.Hubs.Hubs.CommandHandlersHubs;
using NewAppChatSS.Hubs.Infrastructure.Validators;
using NewAppChatSS.Hubs.Interfaces.HubInterfaces;
using NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;
using NewAppChatSS.Hubs.ModelHandlers;
using NewAppChatSS.Hubs.YouTubeAPI;
using NewApplicationChatSS.Mappings;

namespace NewApplicationChatSS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddDbContext<NewAppChatSSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<NewAppChatSSContext>();

            services.AddSignalR();
            services.AddRazorPages();

            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<IMutedUserRepository, MutedUserRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IKickedOutRepository, KickedOutRepository>();
            services.AddTransient<IMemberRepository, MemberRepository>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoomService, RoomService>();
            services.AddTransient<IMemberService, MemberService>();
            services.AddTransient<IMessageService, MessageService>();

            services.AddTransient<IUserCommandHandler, UserCommandHandlerHub>();
            services.AddTransient<IRoomCommandHandler, RoomCommandHandlerHub>();
            services.AddTransient<IHelpCommandHandlerHub, HelpCommandHandlerHub>();
            services.AddSingleton<IBotCommandHandlerHub, BotCommandHandlerHub>();
            services.AddSingleton<IMessageHub, MessageHub>();

            services.AddTransient<IUserValidator, UserValidator>();
            services.AddTransient<IRoomValidator, RoomValidator>();

            services.AddTransient<IMessageHandler, MessageHandler>();
            services.AddTransient<IRoomHandler, RoomHandler>();

            services.AddTransient(typeof(YouTubeRequest));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
               });
        }

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
        }
    }
}
