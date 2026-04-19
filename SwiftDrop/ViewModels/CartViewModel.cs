using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public decimal DeliveryFee { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
    }
}