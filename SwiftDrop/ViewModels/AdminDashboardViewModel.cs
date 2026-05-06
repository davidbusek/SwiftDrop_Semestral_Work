using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// View model for the admin dashboard.
    /// Contains platform-wide statistics and a snapshot of the most recent users.
    /// </summary>
    public class AdminDashboardViewModel
    {
        /// <summary>Total number of registered users on the platform.</summary>
        public int TotalUsers { get; set; }

        /// <summary>Total number of restaurant records in the database.</summary>
        public int ActiveRestaurants { get; set; }

        /// <summary>Sum of all order totals created today, in CZK.</summary>
        public decimal DailyRevenue { get; set; }

        /// <summary>The 50 most recently registered users, shown in the admin user table.</summary>
        public List<User> Users { get; set; } = new();
    }
}
