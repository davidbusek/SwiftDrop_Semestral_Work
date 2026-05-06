using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Manages the customer's shopping cart stored in the HTTP session.
    /// </summary>
    public interface ICartService
    {
        /// <summary>Returns all items currently in the session cart.</summary>
        List<CartItem> GetCart();

        /// <summary>
        /// Adds <paramref name="item"/> to the cart.
        /// If an item with the same <see cref="CartItem.MenuItemId"/> already exists,
        /// its quantity is incremented instead of adding a duplicate.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddToCart(CartItem item);

        /// <summary>
        /// Sets the quantity of a specific menu item in the cart.
        /// Removes the item when <paramref name="quantity"/> is zero or less.
        /// </summary>
        /// <param name="menuItemId">ID of the menu item to update.</param>
        /// <param name="quantity">New quantity; use 0 to remove.</param>
        void UpdateQuantity(int menuItemId, int quantity);

        /// <summary>Removes the item identified by <paramref name="menuItemId"/> from the cart.</summary>
        /// <param name="menuItemId">ID of the menu item to remove.</param>
        void RemoveItem(int menuItemId);

        /// <summary>
        /// Calculates the delivery fee for the current cart contents
        /// using the injected <see cref="IDeliveryCostStrategy"/>.
        /// </summary>
        /// <returns>Delivery fee in CZK.</returns>
        decimal GetTotalDeliveryPrice();

        /// <summary>Empties the cart by removing the session key entirely.</summary>
        void ClearCart();
    }
}
