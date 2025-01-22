using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
            

            // Add services to the container.
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Pristup kontekstu za članove

                // Provjera za uloge i kreiranje uloga ako ne postoje
                var roles = new[] { "Admin", "Member" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Provjera i dodavanje admin korisnika
                string email = "admin@admin.com";
                string password = "Test1234,";
                var adminUser = await userManager.FindByEmailAsync(email);
                if (adminUser == null)
                {
                    var user = new IdentityUser { UserName = email, Email = email };
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Admin");
                }

                var members = await context.Member.ToListAsync(); // Dohvatite sve članove
                foreach (var member in members)
                {
                    var user = await userManager.FindByEmailAsync(member.Email);
                    if (user != null)
                    {
                        // Ako je Amount 25.00, dodajte rolu "Member" ako je još nemaju
                        if (member.Amount == 25.00 && !await userManager.IsInRoleAsync(user, "Member"))
                        {
                            await userManager.AddToRoleAsync(user, "Member");
                        }
                        // Ako Amount nije 25.00, uklonite rolu "Member" ako je imaju
                        else if (member.Amount != 25.00 && await userManager.IsInRoleAsync(user, "Member"))
                        {
                            await userManager.RemoveFromRoleAsync(user, "Member");
                        }
                    }
                }
            }

            app.Run();
        }
    }
}
