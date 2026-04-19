using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(CartItem item);
        void UpdateQuantity(int menuItemId, int quantity);
        void RemoveItem(int menuItemId);
        decimal GetTotalDeliveryPrice();
        void ClearCart();
    }
}