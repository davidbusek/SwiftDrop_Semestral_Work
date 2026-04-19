using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>> GetAllAsync();
        Task<Restaurant?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesWithMenuItemsAsync(int restaurantId);
        Task CreateAsync(Restaurant restaurant);
        Task UpdateAsync(Restaurant restaurant);
        Task DeleteAsync(int id);
    }
}
