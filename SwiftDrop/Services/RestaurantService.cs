using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SwiftDrop.Data;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly SwiftDropDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyActiveRestaurants = "active_restaurants_list";

        public RestaurantService(SwiftDropDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<Restaurant>> GetAllActiveRestaurantsAsync()
        {
            // Performance optimization: Check if active restaurants are already in the memory cache
            if (!_cache.TryGetValue(CacheKeyActiveRestaurants, out IEnumerable<Restaurant>? cachedRestaurants))
            {
                // If data is not in cache, fetch it from the database
                cachedRestaurants = await _context.Restaurants.Where(r => r.IsActive.GetValueOrDefault()).ToListAsync();

                // Set cache expiration time
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                _cache.Set(CacheKeyActiveRestaurants, cachedRestaurants, cacheEntryOptions);
            }

            return cachedRestaurants ?? Array.Empty<Restaurant>();
        }

        public async Task<Restaurant?> GetByIdAsync(int id) => await _context.Restaurants.FindAsync(id);

        public async Task<IEnumerable<Category>> GetCategoriesWithMenuItemsAsync(int restaurantId)
        {
            var cacheKey = $"restaurant_categories_{restaurantId}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Category>? cachedCategories))
            {
                cachedCategories = await _context.Categories
                    .Where(c => c.RestaurantId == restaurantId)
                    .Include(c => c.Menuitems)
                    .ToListAsync();

                _cache.Set(cacheKey, cachedCategories, TimeSpan.FromMinutes(15));
            }

            return cachedCategories ?? Array.Empty<Category>();
        }

        public async Task CreateAsync(Restaurant restaurant)
        {
            _context.Add(restaurant);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        public async Task DeleteAsync(int id)
        {
            var res = await _context.Restaurants.FindAsync(id);
            if (res != null)
            {
                _context.Restaurants.Remove(res);
                await _context.SaveChangesAsync();
                InvalidateCache();
            }
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants.ToListAsync();
        }

        private void InvalidateCache()
        {
            // This invalidates the primary cache when the Restaurants DB table changes
            _cache.Remove(CacheKeyActiveRestaurants);
        }
    }
}
