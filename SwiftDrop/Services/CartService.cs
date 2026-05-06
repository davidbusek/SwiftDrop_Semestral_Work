using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Session-backed implementation of <see cref="ICartService"/>.
    /// Cart data is serialized as JSON and stored under a fixed session key.
    /// Delivery cost is delegated to the injected <see cref="IDeliveryCostStrategy"/>.
    /// </summary>
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeliveryCostStrategy _deliveryStrategy;
        private const string CartSessionKey = "SwiftDrop_Cart";

        /// <summary>
        /// Initializes a new instance of <see cref="CartService"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP session.</param>
        /// <param name="deliveryStrategy">Strategy used to compute delivery fees.</param>
        public CartService(IHttpContextAccessor httpContextAccessor, IDeliveryCostStrategy deliveryStrategy)
        {
            _httpContextAccessor = httpContextAccessor;
            _deliveryStrategy = deliveryStrategy;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session;

        /// <inheritdoc/>
        public List<CartItem> GetCart()
        {
            var session = Session;
            if (session == null) return new List<CartItem>();

            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson)) return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        /// <inheritdoc/>
        public void AddToCart(CartItem item)
        {
            var session = Session;
            if (session == null) return;

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(i => i.MenuItemId == item.MenuItemId);

            if (existingItem != null)
                existingItem.Quantity += item.Quantity;
            else
                cart.Add(item);

            session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        /// <inheritdoc/>
        public void UpdateQuantity(int menuItemId, int quantity)
        {
            var session = Session;
            if (session == null) return;

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;
                else
                    cart.Remove(item);

                session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public decimal GetTotalDeliveryPrice()
        {
            var cart = GetCart();
            return _deliveryStrategy.CalculateDeliveryCost(cart);
        }

        /// <inheritdoc/>
        public void ClearCart()
        {
            Session?.Remove(CartSessionKey);
        }
    }
}
