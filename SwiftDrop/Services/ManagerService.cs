using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Models;
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
        /// <summary>
        /// Returns pending orders and active menu items for the manager dashboard,
        /// scoped to the restaurants associated with <paramref name="managerId"/>
        /// via the <c>Address.UserId</c> convention.
        /// </summary>
        /// <param name="managerId">Primary key of the authenticated <c>RestaurantManager</c> user.</param>
        Task<ManagerDashboardViewModel> GetDashboardDataAsync(int managerId);

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

        /// <summary>
        /// Returns all restaurants associated with the manager identified by <paramref name="managerId"/>.
        /// The association is resolved via <c>Address.UserId</c>.
        /// </summary>
        /// <param name="managerId">Primary key of the <c>RestaurantManager</c> user.</param>
        Task<List<Restaurant>> GetManagerRestaurantsAsync(int managerId);

        /// <summary>
        /// Returns all categories belonging to restaurants managed by <paramref name="managerId"/>.
        /// </summary>
        /// <param name="managerId">Primary key of the <c>RestaurantManager</c> user.</param>
        Task<List<Category>> GetManagerCategoriesAsync(int managerId);

        /// <summary>
        /// Creates a new menu category under the restaurant specified in <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Validated form data for the new category.</param>
        Task CreateCategoryAsync(CreateCategoryViewModel model);

        /// <summary>
        /// Creates a new menu item under the category specified in <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Validated form data for the new menu item.</param>
        Task CreateMenuItemAsync(CreateMenuItemViewModel model);
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
        public async Task<ManagerDashboardViewModel> GetDashboardDataAsync(int managerId)
        {
            // Fetch only the sub-orders that belong to this manager's restaurant so that
            // multi-restaurant orders are shown split — each manager sees only their portion.
            var suborders = await _context.Suborders
                .Where(s => s.Restaurant.Address.UserId == managerId
                         && (s.Order.Status == "Pending" || s.Order.Status == "Paid" || s.Order.Status == "PickupsInProgress"))
                .Include(s => s.Order).ThenInclude(o => o.User)
                .Include(s => s.Orderitems).ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(s => s.Order.CreatedAt)
                .ToListAsync();

            // Menu items whose category → restaurant → address belongs to this manager
            var menuItemsList = await _context.Menuitems
                .Where(m => m.Category.Restaurant.Address.UserId == managerId)
                .Include(m => m.Category)
                .OrderByDescending(m => m.Id)
                .Take(100)
                .ToListAsync();

            return new ManagerDashboardViewModel
            {
                PendingOrders = suborders.Count,
                ActiveItems = menuItemsList.Count,
                Suborders = suborders,
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

        /// <inheritdoc/>
        public async Task<List<Restaurant>> GetManagerRestaurantsAsync(int managerId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == managerId)
                .SelectMany(a => a.Restaurants)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Category>> GetManagerCategoriesAsync(int managerId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == managerId)
                .SelectMany(a => a.Restaurants)
                .SelectMany(r => r.Categories)
                .Include(c => c.Restaurant)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task CreateCategoryAsync(CreateCategoryViewModel model)
        {
            var category = new Category
            {
                RestaurantId = model.RestaurantId,
                Name = model.Name,
                DisplayOrder = model.DisplayOrder ?? 0
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task CreateMenuItemAsync(CreateMenuItemViewModel model)
        {
            var item = new Menuitem
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Allergens = model.Allergens,
                WeightOrVolume = model.WeightOrVolume,
                ImageUrl = model.ImageUrl,
                IsAvailable = true
            };

            _context.Menuitems.Add(item);
            await _context.SaveChangesAsync();
        }
    }
}
