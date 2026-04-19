using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    [Authorize(Roles = "RestaurantManager")]
    public class ManagerController : Controller
    {
        private readonly SwiftDropDbContext _context;

        public ManagerController(SwiftDropDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pendingOrdersList = await _context.Orders
                .Where(o => o.Status == "Pending")
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var menuItemsList = await _context.Menuitems
                .OrderByDescending(m => m.Id).Take(50).ToListAsync();

            var model = new ManagerDashboardViewModel
            {
                PendingOrders = pendingOrdersList.Count,
                ActiveItems = menuItemsList.Count,
                Orders = pendingOrdersList,
                MenuItems = menuItemsList
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = "Preparing"; // State pattern transition point demo
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var item = await _context.Menuitems.FindAsync(id);
            if (item != null)
            {
                _context.Menuitems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}