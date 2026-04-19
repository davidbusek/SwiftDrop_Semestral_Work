using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    public class ManagerDashboardViewModel
    {
        public int PendingOrders { get; set; }
        public int ActiveItems { get; set; }

        public List<Order> Orders { get; set; } = new();
        public List<Menuitem> MenuItems { get; set; } = new();
    }
}