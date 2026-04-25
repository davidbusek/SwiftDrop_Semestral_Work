using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
        Task DeleteUserAsync(int id);
    }

    public class AdminService : IAdminService
    {
        private readonly SwiftDropDbContext _context;

        public AdminService(SwiftDropDbContext context)
        {
            _context = context;
        }

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