using System.Collections.Generic;
using System.Linq;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Default delivery cost strategy used by SwiftDrop.
    /// Charges a base fee of 39 CZK for the first restaurant
    /// plus 25 CZK for each additional restaurant in the same order.
    /// </summary>
    public class SwiftDropDeliveryStrategy : IDeliveryCostStrategy
    {
        /// <summary>
        /// Calculates the delivery fee based on the number of unique restaurants
        /// present in <paramref name="items"/>.
        /// </summary>
        /// <param name="items">Cart items to calculate delivery for.</param>
        /// <returns>
        /// <c>0</c> when the cart is empty; otherwise <c>39 + (n-1) * 25</c> CZK,
        /// where <c>n</c> is the number of distinct restaurants.
        /// </returns>
        public decimal CalculateDeliveryCost(IEnumerable<CartItem> items)
        {
            if (items == null || !items.Any())
            {
                return 0m;
            }

            var uniqueRestaurants = items.Select(i => i.RestaurantId).Distinct().Count();
            return 39.00m + (uniqueRestaurants - 1) * 25.00m;
        }
    }
}
