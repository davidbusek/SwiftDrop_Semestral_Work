using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Models;
using SwiftDrop.Services;

namespace SwiftDrop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           // Database Configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<SwiftDropDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // MVC and Pages Registration
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Default Services
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IManagerService, ManagerService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();

            // Cart, Order and Delivery Services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IDeliveryCostStrategy, SwiftDropDeliveryStrategy>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICourierService, CourierService>();

            // Authentication (Cookie-based)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            // State Management & In-Memory Caching (Data Caching)
            builder.Services.AddMemoryCache(); // Dependency Injection for IMemoryCache (RAM Optimization)

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();


            app.UseAuthentication(); 
            app.UseAuthorization();
            app.UseSession(); 

            app.MapStaticAssets();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}