using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly SwiftDropDbContext _context;

        public CourierController(SwiftDropDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = System.DateTime.Today;

            // Dynamický výpočet odměny (např. 80% z poplatku za doručení za dnešní doručené objednávky)
            var todaysDeliveryFees = await _context.Orders
                .Where(o => o.Status == "Delivered" && o.DeliveredAt >= today)
                .SumAsync(o => (decimal?)o.DeliveryFee) ?? 0m;

            var courierEarnings = todaysDeliveryFees * 0.8m;

            var availableOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Suborders)
                    .ThenInclude(s => s.Orderitems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == "CourierAssigned" || o.Status == "Delivering")
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

            var model = new CourierDashboardViewModel
            {
                ActiveJobs = availableOrders.Count,
                CurrentEarnings = courierEarnings,
                AvailableOrders = availableOrders
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeliveryState(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.Status == "CourierAssigned")
                {
                    order.Status = "Delivering";
                }
                else if (order.Status == "Delivering")
                {
                    order.Status = "Delivered";
                    order.DeliveredAt = System.DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}