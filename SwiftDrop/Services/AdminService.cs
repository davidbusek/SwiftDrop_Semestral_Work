using System;
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
    /// Provides administrative operations such as platform-wide statistics,
    /// user management, restaurant management and menu item management.
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Aggregates platform statistics and returns the latest users, orders,
        /// restaurants and menu items for the admin dashboard.
        /// </summary>
        Task<AdminDashboardViewModel> GetDashboardDataAsync();

        /// <summary>
        /// Permanently deletes the user identified by <paramref name="id"/>.
        /// Has no effect if the user does not exist.
        /// </summary>
        /// <param name="id">Primary key of the user to delete.</param>
        Task DeleteUserAsync(int id);

        /// <summary>
        /// Creates a new <c>RestaurantManager</c> account from the supplied form data.
        /// The password is hashed with BCrypt before storage.
        /// </summary>
        /// <param name="model">Validated form data for the new manager.</param>
        /// <returns><c>true</c> if the account was created; <c>false</c> if the email is already in use.</returns>
        Task<bool> CreateManagerAsync(CreateManagerViewModel model);

        /// <summary>
        /// Creates a new menu item under the category specified in <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Validated form data for the new menu item.</param>
        Task CreateMenuItemAsync(CreateMenuItemViewModel model);

        /// <summary>
        /// Permanently deletes the menu item identified by <paramref name="id"/>
        /// along with any order line items that reference it.
        /// Has no effect if the item does not exist.
        /// </summary>
        /// <param name="id">Primary key of the menu item to delete.</param>
        Task DeleteMenuItemAsync(int id);

        /// <summary>
        /// Permanently deletes the restaurant identified by <paramref name="id"/>
        /// together with its categories and menu items (cascade).
        /// Has no effect if the restaurant does not exist.
        /// </summary>
        /// <param name="id">Primary key of the restaurant to delete.</param>
        Task DeleteRestaurantAsync(int id);

        /// <summary>
        /// Advances the order identified by <paramref name="id"/> to its next state
        /// using the State Pattern (<see cref="SwiftDrop.Services.OrderStates.OrderStateFactory"/>).
        /// Has no effect if the order does not exist or is in a terminal state.
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        Task AdvanceOrderStateAsync(int id);

        /// <summary>
        /// Creates a new restaurant together with its physical address.
        /// The address <c>UserId</c> is set to <paramref name="model"/>.<see cref="CreateRestaurantViewModel.ManagerId"/>,
        /// which is the schema's convention for linking a restaurant to its managing user.
        /// </summary>
        /// <param name="model">Validated form data for the new restaurant.</param>
        Task CreateRestaurantAsync(CreateRestaurantViewModel model);

        /// <summary>
        /// Creates a new menu category under the restaurant specified in <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Validated form data for the new category.</param>
        Task CreateCategoryAsync(CreateCategoryViewModel model);
    }

    /// <summary>EF Core implementation of <see cref="IAdminService"/>.</summary>
    public class AdminService : IAdminService
    {
        private readonly SwiftDropDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="AdminService"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        public AdminService(SwiftDropDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var today = DateTime.Today;

            return new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveRestaurants = await _context.Restaurants.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                DailyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= today)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m,
                TotalRevenue = await _context.Orders
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m,
                Users = await _context.Users
                    .OrderByDescending(u => u.Id).Take(50).ToListAsync(),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Address)
                    .OrderByDescending(o => o.CreatedAt).Take(50).ToListAsync(),
                Restaurants = await _context.Restaurants
                    .Include(r => r.Address)
                    .OrderByDescending(r => r.Id).ToListAsync(),
                MenuItems = await _context.Menuitems
                    .Include(m => m.Category)
                    .ThenInclude(c => c.Restaurant)
                    .OrderByDescending(m => m.Id).Take(100).ToListAsync()
            };
        }

        /// <inheritdoc/>
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CreateManagerAsync(CreateManagerViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return false;

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "RestaurantManager",
                RegisteredAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
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

        /// <inheritdoc/>
        public async Task DeleteMenuItemAsync(int id)
        {
            var item = await _context.Menuitems
                .Include(m => m.Orderitems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item != null)
            {
                // Remove referencing order line items first; MenuItemId is NOT NULL so
                // the DB cannot set it to null — we must delete the rows explicitly.
                _context.Orderitems.RemoveRange(item.Orderitems);
                _context.Menuitems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task DeleteRestaurantAsync(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Categories)
                .ThenInclude(c => c.Menuitems)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }
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
        public async Task CreateRestaurantAsync(CreateRestaurantViewModel model)
        {
            // Address.UserId = ManagerId is the schema's convention for associating
            // a restaurant with its managing user (Restaurant → Address → User).
            var address = new Address
            {
                Street = model.Street,
                City = model.City,
                ZipCode = model.ZipCode,
                UserId = model.ManagerId
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            var restaurant = new Restaurant
            {
                Name = model.Name,
                Description = model.Description,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                LogoUrl = model.LogoUrl,
                EstimatedPrepTimeMinutes = model.EstimatedPrepTimeMinutes,
                MinimumOrderAmount = model.MinimumOrderAmount,
                AddressId = address.Id,
                IsActive = true,
                IsAcceptingOrders = true,
                AverageRating = 0m,
                ReviewCount = 0
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
        }
    }
}
