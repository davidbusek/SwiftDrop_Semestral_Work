using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly SwiftDropDbContext _context;
        public RestaurantService(SwiftDropDbContext context) => _context = context;

        public async Task<IEnumerable<Restaurant>> GetAllActiveRestaurantsAsync()
        {
            return await _context.Restaurants.Where(r => r.IsActive.GetValueOrDefault()).ToListAsync();
        }

        public async Task<Restaurant?> GetByIdAsync(int id) => await _context.Restaurants.FindAsync(id);

        public async Task CreateAsync(Restaurant restaurant)
        {
            _context.Add(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var res = await _context.Restaurants.FindAsync(id);
            if (res != null)
            {
                _context.Restaurants.Remove(res);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants.ToListAsync();
        }
    }
}
