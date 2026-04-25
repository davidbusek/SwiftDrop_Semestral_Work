using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Services
{
    public interface IManagerService
    {
        Task<ManagerDashboardViewModel> GetDashboardDataAsync();
        Task AdvanceOrderStateAsync(int id);
        Task DeleteMenuItemAsync(int id);
    }

    public class ManagerService : IManagerService
    {
        private readonly SwiftDropDbContext _context;

        public ManagerService(SwiftDropDbContext context)
        {
            _context = context;
        }

        public async Task<ManagerDashboardViewModel> GetDashboardDataAsync()
        {
            var pendingOrdersList = await _context.Orders
                .Where(o => o.Status == "Pending" || o.Status == "Paid" || o.Status == "PickupsInProgress")
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var menuItemsList = await _context.Menuitems
                .OrderByDescending(m => m.Id).Take(50).ToListAsync();

            return new ManagerDashboardViewModel
            {
                PendingOrders = pendingOrdersList.Count,
                ActiveItems = menuItemsList.Count,
                Orders = pendingOrdersList,
                MenuItems = menuItemsList
            };
        }

        public async Task AdvanceOrderStateAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.Status == "Pending" || order.Status == "Paid")
                {
                    order.Status = "PickupsInProgress";
                }
                else if (order.Status == "PickupsInProgress")
                {
                    order.Status = "CourierAssigned";
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var item = await _context.Menuitems.FindAsync(id);
            if (item != null)
            {
                _context.Menuitems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}