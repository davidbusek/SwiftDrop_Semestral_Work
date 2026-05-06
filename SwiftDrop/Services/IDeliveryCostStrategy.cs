using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Strategy Pattern interface for delivery cost calculation.
    /// Implementations can define different pricing rules (flat fee, distance-based, etc.)
    /// without changing the cart or checkout logic.
    /// </summary>
    public interface IDeliveryCostStrategy
    {
        /// <summary>
        /// Calculates the total delivery fee for the given collection of cart items.
        /// </summary>
        /// <param name="items">The items currently in the customer's cart.</param>
        /// <returns>The delivery fee in CZK.</returns>
        decimal CalculateDeliveryCost(IEnumerable<CartItem> items);
    }
}
