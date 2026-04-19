using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveRestaurants { get; set; }
        public decimal DailyRevenue { get; set; }

        public List<User> Users { get; set; } = new();
    }
}