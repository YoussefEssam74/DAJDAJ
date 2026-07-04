using DAJDAJ.DataAccess;
using DAJDAJ.DataAccess.DbIntializer;
using DAJDAJ.DataAccess.Implementation;
using DAJDAJ.Entities.Repositories;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Security.Principal;
namespace DAJDAJ.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"))
                    .EnableSensitiveDataLogging()
                    
                );


            builder.Services.AddIdentity<IdentityUser, IdentityRole>
                (options=>options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(30))
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();




            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = "Google";
            })
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = googleAuthSection["ClientId"];
                    options.ClientSecret = googleAuthSection["ClientSecret"];
                });








            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUntiOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbIntializar,DbIntializar>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            builder.Services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "Antiforgery";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

          
            else
            {
                // ✅ Show detailed errors in development/testing
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            app.UseStaticFiles();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
            });


            app.UseRouting();
            SeedDb();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();

            void SeedDb()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbIntializar>();
                    dbInitializer.Initialize();
                }
            }

        }
    }
}
