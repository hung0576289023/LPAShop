using Microsoft.EntityFrameworkCore;
using LPAShop.NET06.Data;
using LPAShop.NET06.Controllers;

namespace LPAShop.NET06
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            //builder.Services.AddScoped<CartController>();

            // Add service of DBContext EF Core
            builder.Services.AddDbContext<LPAShopDBContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("LPAShopDBConnect")));

            builder.Services.Configure<RouteOptions>(options => {
                options.AppendTrailingSlash = false;        // Th�m d?u / v�o cu?i URL
                options.LowercaseUrls = false;               // url ch? th??ng
                options.LowercaseQueryStrings = false;      // kh�ng b?t query trong url ph?i ch? th??ng
            });


            builder.Services.AddDistributedMemoryCache();


            // C?u h�nh cho session, ?? d�ng session ?? l?u tr? d? li?u trong 1 phi�n l�m vi?c
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); ;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // C?u h�nh session
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "MyCustomRoute",
                    pattern: "chi-tiet-san-pham/{id?}",
                    defaults: new { controller = "Product", action = "Detail" }
                );

                // ...
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}