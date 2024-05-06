using Chat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace Chat.Infrastructe.ChatContext.ChatContextSeed
{
    public class IdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {

            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    UserName = "AhmedSamir",
                    FirstName = "Ahmed",
                    LastName = "Samir",
                    Email = "ahmed@gmail.com",
                    City = "Cairo",
                    Country = "Egypt",
                    DateOfBirth = new DateTime(2002, 7, 20),
                    KnownAs = "Abo Sakr",
                    Gender = "Male",
                    Interests = "Programming, Reading",
                    Introduction = "Hello, I'm a new user.",
                    LookingFor = "Looking for something...",
                    EmailConfirmed=true,
                };

                await userManager.CreateAsync(user, "123456");
                await AddUserToRoleAsync(userManager, user.Id, "Admin");

            }

        }


        public static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

        }
        public static async Task AddUserToRoleAsync(UserManager<AppUser> userManager, string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

    }



}
