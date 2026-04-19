using System.Collections.Generic;
using System.Linq;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public class SwiftDropDeliveryStrategy : IDeliveryCostStrategy
    {
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