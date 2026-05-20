using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.Controllers;

// Це приклад контролера, де різні endpoint'и доступні різним ролям.
// [Authorize] — означає "тільки для авторизованих"
// [Authorize(Roles = "Admin")] — тільки для Admin
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // ✅ Доступно всім авторизованим (User, Manager, Admin)
    [HttpGet]
    [Authorize]
    public IActionResult GetAll()
    {
        var products = new[]
        {
            new { Id = 1, Name = "Ноутбук", Price = 30000 },
            new { Id = 2, Name = "Миша",    Price = 500 },
            new { Id = 3, Name = "Клавіатура", Price = 1200 }
        };

        return Ok(products);
    }

    // ✅ Доступно тільки Manager і Admin
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public IActionResult Create([FromBody] object product)
    {
        return Ok(new { message = "Продукт створено (Manager або Admin)", product });
    }

    // ✅ Доступно тільки Manager і Admin
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public IActionResult Update(int id, [FromBody] object product)
    {
        return Ok(new { message = $"Продукт {id} оновлено (Manager або Admin)", product });
    }

    // ✅ Доступно тільки Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        return Ok(new { message = $"Продукт {id} видалено (тільки Admin)" });
    }

    // ❌ Цей endpoint взагалі без захисту (для тесту)
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok(new { message = "Це публічний endpoint, доступний усім" });
    }
}