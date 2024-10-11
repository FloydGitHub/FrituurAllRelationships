using Microsoft.AspNetCore.Authentication.Cookies;
using MVCp2Relatie.Data;


namespace FrituurAllRelationships
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FrituurDb>();

            builder.Services.AddControllersWithViews();

            // voegt Authentication toe, (nog niet toegepast)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Customer/Login";
                    options.LogoutPath = "/Customer/Logout";
                });

            var app = builder.Build();


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }



    }
}
