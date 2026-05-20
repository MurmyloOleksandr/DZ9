using Microsoft.AspNetCore.Identity;

namespace DZ9.Seed;

// Цей клас запускається один раз при старті програми.
// Він створює ролі та тестових користувачів, якщо вони ще не існують.
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // --- Створення ролей ---
        string[] roles = ["Admin", "Manager", "User"];

        foreach (var role in roles)
        {
            // Перевіряємо чи роль вже існує, щоб не дублювати
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"✅ Роль '{role}' створена");
            }
        }

        // --- Створення тестових користувачів ---
        await CreateUser(userManager, "admin@demo.com", "Admin123!", "Admin");
        await CreateUser(userManager, "manager@demo.com", "Manager123!", "Manager");
        await CreateUser(userManager, "user@demo.com", "User123!", "User");
    }

    // Допоміжний метод — створює юзера і видає йому роль
    private static async Task CreateUser(
        UserManager<IdentityUser> userManager,
        string email, string password, string role)
    {
        // Якщо такий email вже є — пропускаємо
        if (await userManager.FindByEmailAsync(email) != null) return;

        var user = new IdentityUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
            Console.WriteLine($"✅ Користувач '{email}' створений з роллю '{role}'");
        }
        else
        {
            // Виводимо помилки, якщо щось пішло не так
            foreach (var error in result.Errors)
                Console.WriteLine($"❌ {error.Description}");
        }
    }
}