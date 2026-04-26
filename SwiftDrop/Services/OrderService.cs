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
    public interface IOrderService
    {
        Task<Order> ProcessCheckoutAsync(string userEmail, List<CartItem> cartItems, decimal deliveryFee, string street, string city, string zipCode);
        Task<bool> MockPaymentProcessAsync(int orderId, decimal amount);
        Task<List<Order>> GetUserOrdersByEmailAsync(string userEmail);
    }

    public class OrderService : IOrderService
    {
        private readonly SwiftDropDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(SwiftDropDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Order>> GetUserOrdersByEmailAsync(string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return new List<Order>();

            return await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> ProcessCheckoutAsync(string userEmail, List<CartItem> cartItems, decimal deliveryFee, string street, string city, string zipCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                throw new Exception("User not found.");

            var itemPrice = cartItems.Sum(i => i.Price * i.Quantity);
            var totalPrice = itemPrice + deliveryFee;

            var deliveryAddress = new Address
            {
                UserId = user.Id,
                Street = street,
                City = city,
                ZipCode = zipCode
            };

            var coords = await GeocodeAddressAsync(street, city);
            deliveryAddress.Latitude = coords?.Lat;
            deliveryAddress.Longitude = coords?.Lng;

            _context.Addresses.Add(deliveryAddress);
            await _context.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                AddressId = deliveryAddress.Id,
                Status = "Pending",
                ItemPrice = itemPrice,
                DeliveryFee = deliveryFee,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Skupinujme položky podle restaurace
            var groupedItems = cartItems.GroupBy(i => i.RestaurantId);
            foreach(var group in groupedItems) 
            {
                var subOrder = new Suborder
                {
                    OrderId = order.Id,
                    RestaurantId = group.Key,
                    Status = "Pending",
                };
                
                _context.Suborders.Add(subOrder);
                await _context.SaveChangesAsync();

                foreach (var item in group)
                {
                    _context.Orderitems.Add(new Orderitem
                    {
                        SubOrderId = subOrder.Id,
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    });
                }
            }

            await _context.SaveChangesAsync();

            return order;
        }

        private async Task<(decimal Lat, decimal Lng)?> GeocodeAddressAsync(string street, string city)
        {
            try
            {
                var query = Uri.EscapeDataString($"{street}, {city}, Czech Republic");
                var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&limit=1";

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("SwiftDrop/1.0");

                var json = await client.GetStringAsync(url);
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

        public async Task<bool> MockPaymentProcessAsync(int orderId, decimal amount)
        {
            // Simulace zdržení na platební bráně
            await Task.Delay(1000);

            // Pseudo-náhodné úspěšné vyhodnocení (např. 90% šance na úspěch)
            var rng = new Random();
            bool paymentSuccess = rng.Next(0, 100) > 10;

            var payment = new Payment
            {
                OrderId = orderId,
                PaymentMethod = "CardOnline",
                PaymentStatus = paymentSuccess ? "Paid" : "Unpaid",
                Amount = amount,
                CreatedAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = paymentSuccess ? "Paid" : "Canceled";
            }

            await _context.SaveChangesAsync();

            return paymentSuccess;
        }
    }
}