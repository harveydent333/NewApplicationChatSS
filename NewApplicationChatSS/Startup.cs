using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.BLL.Hubs;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.BLL.Interfaces.ServiceInterfaces;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.BLL.Services;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.BLL.Infrastructure.Validators;
using NewAppChatSS.DAL.Repositories;
using NewAppChatSS.DAL;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using AutoMapper;
using NewApplicationChatSS.Mappings;
using NewAppChatSS.BLL.Hubs.CommandHandlersHubs;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.BLL.Infrastructure.ModelHandlers;

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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSignalR();
            services.AddRazorPages();

            services.AddTransient<IUnitOfWork, EFUnitOfWork>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoomService, RoomService>();
            services.AddTransient<IMemberService, MemberService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IUserCommandHandler, UserCommandHandlerHub>();
            services.AddTransient<IRoomCommandHandler, RoomCommandHandlerHub>();
            services.AddTransient<IHelpCommandHandlerHub, HelpCommandHandlerHub>();
            services.AddTransient<IBotCommandHandlerHub, BotCommandHandlerHub>();
            services.AddTransient<IUserValidator, UserValidator>();
            services.AddTransient<IRoomValidator, RoomValidator>();
            services.AddTransient<IMessageHandler, MessageHandler>();
            services.AddTransient<IRoomHandler, RoomHandler>();

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

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")
                ),
                RequestPath = "/node_modules",
                EnableDirectoryBrowsing = false
            });

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
