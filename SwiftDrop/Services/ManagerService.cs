using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Services.OrderStates;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Provides restaurant-manager operations: viewing incoming orders,
    /// advancing order states and managing menu items.
    /// </summary>
    public interface IManagerService
    {
        /// <summary>Returns pending orders and active menu items for the manager dashboard.</summary>
        Task<ManagerDashboardViewModel> GetDashboardDataAsync();

        /// <summary>
        /// Advances the order identified by <paramref name="id"/> to its next state
        /// using the State Pattern (<see cref="OrderStateFactory"/>).
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        Task AdvanceOrderStateAsync(int id);

        /// <summary>
        /// Deletes the menu item identified by <paramref name="id"/>.
        /// Has no effect if the item does not exist.
        /// </summary>
        /// <param name="id">Primary key of the menu item to delete.</param>
        Task DeleteMenuItemAsync(int id);
    }

    /// <summary>EF Core implementation of <see cref="IManagerService"/>.</summary>
    public class ManagerService : IManagerService
    {
        private readonly SwiftDropDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="ManagerService"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ManagerService(SwiftDropDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task AdvanceOrderStateAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                var state = OrderStateFactory.GetState(order.Status);
                if (state.CanAdvance)
                    state.Advance(order);
                await _context.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
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
