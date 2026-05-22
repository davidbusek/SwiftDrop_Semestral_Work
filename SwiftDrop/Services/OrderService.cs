using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Models;
using System.Collections.Generic;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Handles order creation, payment processing and order history retrieval.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Creates a complete order from the current cart: reuses or creates a delivery address
        /// (geocoded via Nominatim), then persists the order, sub-orders per restaurant,
        /// and all order items — all within a single database transaction.
        /// </summary>
        /// <param name="userEmail">Email of the authenticated customer.</param>
        /// <param name="cartItems">Items to be ordered.</param>
        /// <param name="deliveryFee">Pre-calculated delivery fee in CZK.</param>
        /// <param name="street">Delivery street.</param>
        /// <param name="city">Delivery city.</param>
        /// <param name="zipCode">Delivery ZIP code.</param>
        /// <returns>The newly created <see cref="Order"/>.</returns>
        Task<Order> ProcessCheckoutAsync(string userEmail, List<CartItem> cartItems, decimal deliveryFee,
            string street, string city, string zipCode);

        /// <summary>
        /// Simulates a card payment with a 90 % success rate.
        /// Persists a <see cref="Payment"/> record and updates the order status to
        /// <c>Paid</c> or <c>Canceled</c> accordingly.
        /// </summary>
        /// <param name="orderId">The order to pay for.</param>
        /// <param name="amount">Amount charged in CZK.</param>
        /// <returns><c>true</c> if payment succeeded; <c>false</c> otherwise.</returns>
        Task<bool> MockPaymentProcessAsync(int orderId, decimal amount);

        /// <summary>Returns all orders placed by the user with the given email, newest first.</summary>
        /// <param name="userEmail">Email of the user whose orders to retrieve.</param>
        Task<List<Order>> GetUserOrdersByEmailAsync(string userEmail);

        /// <summary>
        /// Returns a single order with its sub-orders and items included,
        /// or <c>null</c> if the order does not belong to <paramref name="userEmail"/>.
        /// </summary>
        /// <param name="orderId">Primary key of the order to track.</param>
        /// <param name="userEmail">Email of the authenticated customer — used to verify ownership.</param>
        Task<Order?> GetOrderForTrackingAsync(int orderId, string userEmail);
    }

    /// <summary>EF Core implementation of <see cref="IOrderService"/>.</summary>
    public class OrderService : IOrderService
    {
        private readonly SwiftDropDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="OrderService"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="httpClientFactory">Factory used to call the Nominatim geocoding API.</param>
        public OrderService(SwiftDropDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public async Task<List<Order>> GetUserOrdersByEmailAsync(string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return new List<Order>();

            return await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Order> ProcessCheckoutAsync(string userEmail, List<CartItem> cartItems,
            decimal deliveryFee, string street, string city, string zipCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) throw new Exception("User not found.");

            // Start geocoding immediately — runs concurrently with the DB operations below
            var geocodeTask = GeocodeAddressAsync(street, city);

            var itemPrice = cartItems.Sum(i => i.Price * i.Quantity);
            var totalPrice = itemPrice + deliveryFee;

            Order order;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Reuse an existing address if the user has already ordered to the same location
                var deliveryAddress = await _context.Addresses.FirstOrDefaultAsync(a =>
                    a.UserId == user.Id &&
                    a.Street == street &&
                    a.City == city &&
                    a.ZipCode == zipCode);

                if (deliveryAddress == null)
                {
                    deliveryAddress = new Address
                    {
                        UserId = user.Id,
                        Street = street,
                        City = city,
                        ZipCode = zipCode
                    };
                    _context.Addresses.Add(deliveryAddress);
                    await _context.SaveChangesAsync();
                }

                order = new Order
                {
                    UserId = user.Id,
                    AddressId = deliveryAddress.Id,
                    Status = OrderStatus.Pending,
                    ItemPrice = itemPrice,
                    DeliveryFee = deliveryFee,
                    TotalPrice = totalPrice,
                    CreatedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var group in cartItems.GroupBy(i => i.RestaurantId))
                {
                    var subOrder = new SubOrder
                    {
                        OrderId = order.Id,
                        RestaurantId = group.Key,
                        Status = SubOrderStatus.Pending
                    };

                    _context.SubOrders.Add(subOrder);
                    await _context.SaveChangesAsync();

                    foreach (var item in group)
                    {
                        _context.OrderItems.Add(new OrderItem
                        {
                            SubOrderId = subOrder.Id,
                            MenuItemId = item.MenuItemId,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Geocoding finishes here (usually already done by the time DB work completes)
                var coords = await geocodeTask;
                if (coords.HasValue)
                {
                    deliveryAddress.Latitude = coords.Value.Lat;
                    deliveryAddress.Longitude = coords.Value.Lng;
                }
                else
                {
                    // Fallback coordinates (Prague center) if geocoding fails or address is invalid
                    deliveryAddress.Latitude = 50.08804m;
                    deliveryAddress.Longitude = 14.42076m;
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return order;
        }

        /// <summary>
        /// Geocodes a Czech address using the OpenStreetMap Nominatim API.
        /// Returns <c>null</c> silently on any error — geocoding failure is non-fatal.
        /// </summary>
        private async Task<(decimal Lat, decimal Lng)?> GeocodeAddressAsync(string street, string city)
        {
            try
            {
                var query = Uri.EscapeDataString($"{street}, {city}, Czech Republic");
                var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&limit=1";

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("SwiftDrop_UniversityProject/1.0 (student@sssvt.cz)");

                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
                var json = await client.GetStringAsync(url, cts.Token);
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.GetArrayLength() > 0)
                {
                    var first = root[0];
                    var lat = decimal.Parse(first.GetProperty("lat").GetString()!, System.Globalization.CultureInfo.InvariantCulture);
                    var lng = decimal.Parse(first.GetProperty("lon").GetString()!, System.Globalization.CultureInfo.InvariantCulture);
                    return (lat, lng);
                }
            }
            catch { /* geocoding failure is non-fatal */ }

            return null;
        }

        /// <inheritdoc/>
        public async Task<Order?> GetOrderForTrackingAsync(int orderId, string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return null;

            return await _context.Orders
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.Restaurant)
                .Include(o => o.SubOrders)
                    .ThenInclude(s => s.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);
        }

        /// <inheritdoc/>
        public async Task<bool> MockPaymentProcessAsync(int orderId, decimal amount)
        {
            await Task.Delay(1000);

            bool paymentSuccess = new Random().Next(0, 100) > 10;

            _context.Payments.Add(new Payment
            {
                OrderId = orderId,
                PaymentMethod = "CardOnline",
                PaymentStatus = paymentSuccess ? "Paid" : "Unpaid",
                Amount = amount,
                CreatedAt = DateTime.Now
            });

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
                order.Status = paymentSuccess ? OrderStatus.Paid : OrderStatus.Canceled;

            await _context.SaveChangesAsync();
            return paymentSuccess;
        }
    }
}
