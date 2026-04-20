using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    public class MyOrdersViewModel
    {
        public List<Order> Orders { get; set; } = new();
    }
}