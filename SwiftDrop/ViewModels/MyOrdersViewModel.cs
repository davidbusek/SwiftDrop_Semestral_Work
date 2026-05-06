using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>View model for the customer's order history page.</summary>
    public class MyOrdersViewModel
    {
        /// <summary>All orders placed by the logged-in customer, newest first.</summary>
        public List<Order> Orders { get; set; } = new();
    }
}
