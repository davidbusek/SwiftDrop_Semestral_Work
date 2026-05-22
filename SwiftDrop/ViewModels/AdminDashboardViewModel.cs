using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// View model for the admin dashboard.
    /// Contains platform-wide statistics and snapshots of users, orders, restaurants and menu items.
    /// </summary>
    public class AdminDashboardViewModel
    {
        /// <summary>Total number of registered users on the platform.</summary>
        public int TotalUsers { get; set; }

        /// <summary>Total number of restaurant records in the database.</summary>
        public int ActiveRestaurants { get; set; }

        /// <summary>Sum of all order totals created today, in CZK.</summary>
        public decimal DailyRevenue { get; set; }

        /// <summary>Total number of orders ever placed on the platform.</summary>
        public int TotalOrders { get; set; }

        /// <summary>All-time sum of completed order totals, in CZK.</summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>The 50 most recently registered users, shown in the admin user table.</summary>
        public List<User> Users { get; set; } = new();

        /// <summary>The 50 most recent orders across all customers, newest first.</summary>
        public List<Order> RecentOrders { get; set; } = new();

        /// <summary>All restaurants registered on the platform.</summary>
        public List<Restaurant> Restaurants { get; set; } = new();

        /// <summary>The 100 most recently added menu items across all restaurants.</summary>
        public List<MenuItem> MenuItems { get; set; } = new();
    }
}
