using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// View model for the restaurant manager dashboard.
    /// Aggregates sub-orders awaiting manager action and the current menu item listing.
    /// Each <see cref="Suborder"/> represents this manager's portion of a customer order
    /// and includes only the items belonging to their restaurant.
    /// </summary>
    public class ManagerDashboardViewModel
    {
        /// <summary>Count of sub-orders in <c>Pending</c>, <c>Paid</c> or <c>PickupsInProgress</c> parent-order state.</summary>
        public int PendingOrders { get; set; }

        /// <summary>Number of menu items shown in the active items table.</summary>
        public int ActiveItems { get; set; }

        /// <summary>
        /// Sub-orders for this manager's restaurant requiring attention, ordered newest first.
        /// Each entry exposes its parent <see cref="Order"/> (with <see cref="Order.User"/>) and its own <see cref="Suborder.Orderitems"/>.
        /// </summary>
        public List<Suborder> Suborders { get; set; } = new();

        /// <summary>The 100 most recently added menu items for this manager's restaurants.</summary>
        public List<Menuitem> MenuItems { get; set; } = new();
    }
}
