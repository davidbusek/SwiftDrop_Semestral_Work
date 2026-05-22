using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// View model for the cart summary page.
    /// Aggregates cart items with pre-calculated totals so the view requires no arithmetic.
    /// </summary>
    public class CartViewModel
    {
        /// <summary>All items currently in the session cart.</summary>
        public List<CartItem> CartItems { get; set; } = new();

        /// <summary>Delivery fee in CZK calculated by the active <see cref="SwiftDrop.Services.IDeliveryCostStrategy"/>.</summary>
        public decimal DeliveryFee { get; set; }

        /// <summary>Sum of (unit price × quantity) for all items, in CZK.</summary>
        public decimal Subtotal { get; set; }

        /// <summary>Grand total: <see cref="Subtotal"/> + <see cref="DeliveryFee"/>.</summary>
        public decimal Total { get; set; }

        /// <summary>Saved delivery addresses belonging to the authenticated user. Empty for guests.</summary>
        public List<Address> SavedAddresses { get; set; } = new();
    }
}
