using LostFindingApi.Models;
using LostFindingApi.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostFindingApi.Services
{
    public class ContextSeedService
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ContextSeedService(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            if(context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            if(!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SD.UserRole });
                await roleManager.CreateAsync(new IdentityRole { Name = SD.AdminRole });
            }

            if (!userManager.Users.AnyAsync().GetAwaiter().GetResult())
            {
                var admin = new User
                {
                    Email = "hamedMostafa726@gmail.com",
                    UserName = "MostafaHamed",
                    EmailConfirmed = true,
                    City = "Alexandria",
                    region = "Miami",
                    PhoneNumber = "01014972956",
                };

                await userManager.CreateAsync(admin, "Mostafa+Seffy@21");
                await userManager.AddToRoleAsync(admin, SD.AdminRole);
                await userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(ClaimTypes.Email,admin.Email),
                });
            }
        }
    }
}
