using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
// using SwiftDrop.Data;
// using SwiftDrop.Models;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly SwiftDropDbContext _context;

    public HomeController(SwiftDropDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetOverview()
    {
        // Vrátíme základní info - kolik máme restaurací a aktivních objednávek
        var stats = new
        {
            RestaurantCount = await _context.Restaurants.CountAsync(),
            ActiveOrders = await _context.Orders.CountAsync(o => o.Status != "Delivered"),
            // Tady uvidíš ty testovací data, co jsme tam vložili přes SQL
            LatestRestaurants = await _context.Restaurants.Take(5).ToListAsync()
        };

        return Ok(stats);
    }
}