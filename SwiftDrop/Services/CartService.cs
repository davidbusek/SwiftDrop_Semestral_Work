using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeliveryCostStrategy _deliveryStrategy;
        private const string CartSessionKey = "SwiftDrop_Cart";

        public CartService(IHttpContextAccessor httpContextAccessor, IDeliveryCostStrategy deliveryStrategy)
        {
            _httpContextAccessor = httpContextAccessor;
            _deliveryStrategy = deliveryStrategy;
        }

        private ISession Session
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Session;
            }
        }

        public List<CartItem> GetCart()
        {
            var session = Session;
            if (session == null)
            {
                return new List<CartItem>();
            }

            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        public void AddToCart(CartItem item)
        {
            var session = Session;
            if (session == null)
            {
                return;
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(i => i.MenuItemId == item.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public void UpdateQuantity(int menuItemId, int quantity)
        {
            var session = Session;
            if (session == null) return;

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Remove(item);
                }
                session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
            }
        }

        public void RemoveItem(int menuItemId)
        {
            var session = Session;
            if (session == null) return;

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (item != null)
            {
                cart.Remove(item);
                session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
            }
        }

        public decimal GetTotalDeliveryPrice()
        {
            var cart = GetCart();
            return _deliveryStrategy.CalculateDeliveryCost(cart);
        }

        public void ClearCart()
        {
            var session = Session;
            if (session != null)
            {
                session.Remove(CartSessionKey);
            }
        }
    }
}