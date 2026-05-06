using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// View model for the restaurant manager dashboard.
    /// Aggregates orders awaiting manager action and the current menu item listing.
    /// </summary>
    public class ManagerDashboardViewModel
    {
        /// <summary>Count of orders in <c>Pending</c>, <c>Paid</c> or <c>PickupsInProgress</c> state.</summary>
        public int PendingOrders { get; set; }

        /// <summary>Number of menu items shown in the active items table.</summary>
        public int ActiveItems { get; set; }

        /// <summary>Orders requiring manager attention, ordered newest first.</summary>
        public List<Order> Orders { get; set; } = new();

        /// <summary>The 50 most recently added menu items.</summary>
        public List<Menuitem> MenuItems { get; set; } = new();
    }
}
