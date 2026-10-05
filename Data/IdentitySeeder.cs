using Microsoft.AspNetCore.Identity;

namespace PariniFSL.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager =
            serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roleRenames = new Dictionary<string, string>
    {
        { "Student", "Studente" },
        { "Teacher", "Docente tutor" },
        { "Referent", "Referente" }
    };

        foreach (var rename in roleRenames)
        {
            var oldRole = await roleManager.FindByNameAsync(rename.Key);
            var newRole = await roleManager.FindByNameAsync(rename.Value);

            if (oldRole != null && newRole == null)
            {
                oldRole.Name = rename.Value;
                oldRole.NormalizedName = rename.Value.ToUpperInvariant();

                await roleManager.UpdateAsync(oldRole);
            }
        }

        string[] roles =
        {
        "Studente",
        "Docente tutor",
        "Referente",
        "Admin"
    };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }


    public static async Task SeedAdminAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var userManager =
            serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "AdminUser:Email e AdminUser:Password devono essere configurati.");
        }

        var existingAdmin = await userManager.FindByEmailAsync(email);

        Console.WriteLine($"ADMIN EMAIL: {email}");
        Console.WriteLine($"ADMIN ESISTE: {existingAdmin != null}");


        if (existingAdmin == null)
        {
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Impossibile creare l'Admin: {errors}");
            }

            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(existingAdmin, "Admin");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);

            var resetResult = await userManager.ResetPasswordAsync(
                existingAdmin,
                token,
                password);

            if (!resetResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    resetResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Impossibile reimpostare la password dell'Admin: {errors}");
            }

            Console.WriteLine($"RESET PASSWORD RIUSCITO: {resetResult.Succeeded}");

        }

    }
}
