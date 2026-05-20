using AuthDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    // UserManager — керує користувачами (створення, пошук, ролі)
    // SignInManager — керує сесіями (вхід, вихід)
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // POST /api/account/register
    // Реєстрація нового користувача
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        // Створюємо користувача з паролем
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // За замовчуванням новий юзер отримує роль "User"
        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "Реєстрація успішна! Роль: User" });
    }

    // POST /api/account/login
    // Логін — встановлює cookie-сесію
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // isPersistent: false — сесія закінчується при закритті браузера
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized(new { message = "Невірний email або пароль" });

        // Отримуємо ролі користувача для відповіді
        var user = await _userManager.FindByEmailAsync(model.Email);
        var roles = await _userManager.GetRolesAsync(user!);

        return Ok(new { message = "Вхід успішний", roles });
    }

    // POST /api/account/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Вихід успішний" });
    }

    // GET /api/account/me
    // Повертає інформацію про поточного авторизованого користувача
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized(new { message = "Ви не авторизовані" });

        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user!);

        return Ok(new { user!.Email, roles });
    }
}