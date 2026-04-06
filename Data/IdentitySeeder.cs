using Microsoft.AspNetCore.Identity;

namespace SleepFactorsApp.Data;

public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string SeedAdminUserName = "adminuser";
    public const string SeedAdminPassword = "Vijaya108!";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        if (!await roleManager.RoleExistsAsync(UserRole))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRole));
        }

        var adminUser = await userManager.FindByNameAsync(SeedAdminUserName);
        if (adminUser is null)
        {
            adminUser = new IdentityUser
            {
                UserName = SeedAdminUserName,
                Email = "adminuser@sleepfactors.local",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, SeedAdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to create seed admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
        {
            await userManager.AddToRoleAsync(adminUser, AdminRole);
        }
    }
}
