using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Provides CRUD operations and read queries for <see cref="Restaurant"/> data.
    /// Implementations may add caching on top of the database layer.
    /// </summary>
    public interface IRestaurantService
    {
        /// <summary>Returns all restaurants regardless of active status.</summary>
        Task<IEnumerable<Restaurant>> GetAllAsync();

        /// <summary>
        /// Returns the restaurant with the given <paramref name="id"/>,
        /// or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">Primary key of the restaurant.</param>
        Task<Restaurant?> GetByIdAsync(int id);

        /// <summary>
        /// Returns all menu categories (with their items) for the given restaurant.
        /// </summary>
        /// <param name="restaurantId">Primary key of the restaurant.</param>
        Task<IEnumerable<Category>> GetCategoriesWithMenuItemsAsync(int restaurantId);

        /// <summary>Persists a new restaurant to the database and invalidates the cache.</summary>
        /// <param name="restaurant">The restaurant to create.</param>
        Task CreateAsync(Restaurant restaurant);

        /// <summary>Updates an existing restaurant record and invalidates the cache.</summary>
        /// <param name="restaurant">The restaurant with updated values.</param>
        Task UpdateAsync(Restaurant restaurant);

        /// <summary>Deletes the restaurant identified by <paramref name="id"/> and invalidates the cache.</summary>
        /// <param name="id">Primary key of the restaurant to delete.</param>
        Task DeleteAsync(int id);
    }
}
