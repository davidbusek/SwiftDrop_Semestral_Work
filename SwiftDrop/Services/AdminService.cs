using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Provides administrative operations such as platform-wide statistics
    /// and user management.
    /// </summary>
    public interface IAdminService
    {
        /// <summary>Aggregates platform statistics and returns the latest users for the admin dashboard.</summary>
        Task<AdminDashboardViewModel> GetDashboardDataAsync();

        /// <summary>
        /// Permanently deletes the user identified by <paramref name="id"/>.
        /// Has no effect if the user does not exist.
        /// </summary>
        /// <param name="id">Primary key of the user to delete.</param>
        Task DeleteUserAsync(int id);
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
                DailyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= today)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m,
                Users = await _context.Users.OrderByDescending(u => u.Id).Take(50).ToListAsync()
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
    }
}
