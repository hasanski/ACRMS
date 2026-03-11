using Microsoft.AspNetCore.Identity;

namespace ACRMS.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@acrms.com";
            const string adminPassword = "Admin@123";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin is null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    UniversityNumber = "AABU001",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
                else
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Admin user creation failed: {errors}");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(existingAdmin, Roles.Admin))
                {
                    await userManager.AddToRoleAsync(existingAdmin, Roles.Admin);
                }
            }
        }
    }
}