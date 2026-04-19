using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly SwiftDropDbContext _context;

        public AdminController(SwiftDropDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveRestaurants = await _context.Restaurants.CountAsync(),
                DailyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= today)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m,
                Users = await _context.Users.OrderByDescending(u => u.Id).Take(50).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}