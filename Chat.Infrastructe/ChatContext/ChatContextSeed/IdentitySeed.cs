using Chat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace Chat.Infrastructe.ChatContext.ChatContextSeed
{
    public class IdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {

            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    UserName = "AhmedSamir",
                    FirstName="Ahmed",
                    LastName="Samir",
                    Email = "ahmed@gmail.com",
                    City = "Cairo",
                    Country = "Egypt",
                    DateOfBirth = new DateTime(2002, 7, 20),
                    KnownAs = "Abo Sakr",
                    Gender="Male",
                    Interests = "Programming, Reading" ,
                    Introduction = "Hello, I'm a new user.",
                    LookingFor = "Looking for something...",
                   
                };

                var result = await userManager.CreateAsync(user, "Aa123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }

            if (!roleManager.Roles.Any())
            {
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole(){ Name="Admin"},
                    new IdentityRole(){ Name="Moderator"},
                    new IdentityRole(){ Name="Menmber"}
                };
                foreach (var role in roles)
                    await roleManager.CreateAsync(role);
            }
      
        }
    }
}
