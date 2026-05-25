using Chapeau.Repositories;
using Chapeau.Services;

namespace Chapeau
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IOrderRepository, DbOrderRepository>();
            builder.Services.AddScoped<IOrderServices, OrderService>();
            builder.Services.AddScoped<IMenuListRepository, DbMenuListRepository>();
            builder.Services.AddScoped<IMenuService, MenuService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<ITablesService, TableService>();
            builder.Services.AddScoped<IUserService, UserServices>();
            

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddScoped<IMenuListService, MenuListService>();
            builder.Services.AddScoped<IMenuRepository, MenuRepository>();
            builder.Services.AddSession();
            

            builder.Services.AddControllersWithViews();

            builder.Services.AddSession();
            var app = builder.Build();
            app.UseSession();

            
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}