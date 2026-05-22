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
    /// <summary>
    /// EF Core + in-memory cache implementation of <see cref="IRestaurantService"/>.
    /// Frequently read lists are cached for 15 minutes (Cache-Aside Pattern) and
    /// invalidated on any write operation.
    /// </summary>
    public class RestaurantService : IRestaurantService
    {
        private readonly SwiftDropDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyActiveRestaurants = "active_restaurants_list";

        /// <summary>
        /// Initializes a new instance of <see cref="RestaurantService"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="cache">In-memory cache for reducing DB round-trips.</param>
        public RestaurantService(SwiftDropDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Returns all active restaurants. Result is cached for 15 minutes.
        /// </summary>
        public async Task<IEnumerable<Restaurant>> GetAllActiveRestaurantsAsync()
        {
            if (!_cache.TryGetValue(CacheKeyActiveRestaurants, out IEnumerable<Restaurant>? cachedRestaurants))
            {
                cachedRestaurants = await _context.Restaurants
                    .Where(r => r.IsActive.GetValueOrDefault())
                    .ToListAsync();

                _cache.Set(CacheKeyActiveRestaurants, cachedRestaurants,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
            }

            return cachedRestaurants ?? Array.Empty<Restaurant>();
        }

        /// <inheritdoc/>
        public async Task<Restaurant?> GetByIdAsync(int id) =>
            await _context.Restaurants.FindAsync(id);

        /// <summary>
        /// Returns all menu categories with their items for the given restaurant.
        /// Result is cached per restaurant for 15 minutes.
        /// </summary>
        /// <param name="restaurantId">Primary key of the restaurant.</param>
        public async Task<IEnumerable<Category>> GetCategoriesWithMenuItemsAsync(int restaurantId)
        {
            var cacheKey = $"restaurant_categories_{restaurantId}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Category>? cachedCategories))
            {
                cachedCategories = await _context.Categories
                    .Where(c => c.RestaurantId == restaurantId)
                    .Include(c => c.MenuItems)
                    .ToListAsync();

                _cache.Set(cacheKey, cachedCategories, TimeSpan.FromMinutes(15));
            }

            return cachedCategories ?? Array.Empty<Category>();
        }

        /// <inheritdoc/>
        public async Task CreateAsync(Restaurant restaurant)
        {
            _context.Add(restaurant);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            InvalidateCache();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<IEnumerable<Restaurant>> GetAllAsync() =>
            await _context.Restaurants.ToListAsync();

        /// <inheritdoc/>
        public void InvalidateCategoryCache(int restaurantId) =>
            _cache.Remove($"restaurant_categories_{restaurantId}");

        /// <summary>Removes the restaurant list cache entry so the next read fetches fresh data.</summary>
        private void InvalidateCache() => _cache.Remove(CacheKeyActiveRestaurants);
    }
}
