using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(CartItem item);
        decimal GetTotalDeliveryPrice();
        void ClearCart();
    }
}