using System;

namespace SwiftDrop.Models
{
    /// <summary>
    /// Represents a single line in the customer's session-based shopping cart.
    /// Instances are serialized to JSON and stored in the HTTP session.
    /// </summary>
    public class CartItem
    {
        /// <summary>Primary key of the underlying <see cref="MenuItem"/>.</summary>
        public int MenuItemId { get; set; }

        /// <summary>Primary key of the restaurant this item belongs to.</summary>
        public int RestaurantId { get; set; }

        /// <summary>Display name of the menu item.</summary>
        public string Name { get; set; } = null!;

        /// <summary>Unit price in CZK at the time the item was added to the cart.</summary>
        public decimal Price { get; set; }

        /// <summary>Number of units ordered.</summary>
        public int Quantity { get; set; }

        /// <summary>URL of the menu item's image, used on the cart page.</summary>
        public string? ImageUrl { get; set; }
    }
}
