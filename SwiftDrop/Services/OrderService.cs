using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Models;
using System.Collections.Generic;

namespace SwiftDrop.Services
{
    public interface IOrderService
    {
        Task<Order> ProcessCheckoutAsync(string userEmail, List<CartItem> cartItems, decimal deliveryFee);
        Task<bool> MockPaymentProcessAsync(int orderId, decimal amount);
    }

    public class OrderService : IOrderService
    {
        private readonly SwiftDropDbContext _context;

        public OrderService(SwiftDropDbContext context)
        {
            _context = context;
        }

        public async Task<Order> ProcessCheckoutAsync(string userEmail, List<CartItem> cartItems, decimal deliveryFee)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var itemPrice = cartItems.Sum(i => i.Price * i.Quantity);
            var totalPrice = itemPrice + deliveryFee;

            // In a real application, AddressId should come from user selection.
            // Using a default/mock address ID for Demo/Semestral.
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == user.Id);
            int addressId = address?.Id ?? 1; 

            var order = new Order
            {
                UserId = user.Id,
                AddressId = addressId,
                Status = "Payment Pending",
                ItemPrice = itemPrice,
                DeliveryFee = deliveryFee,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Skupinujme položky podle restaurace, i když naše UI neumožňuje mix restaurací
            var subOrder = new Suborder
            {
                OrderId = order.Id,
                RestaurantId = cartItems.First().RestaurantId,
                Status = "Pending",
            };
            
            _context.Suborders.Add(subOrder);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                _context.Orderitems.Add(new Orderitem
                {
                    SubOrderId = subOrder.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });
            }

            await _context.SaveChangesAsync();

            return order;
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
                PaymentMethod = "CyberCredit",
                PaymentStatus = paymentSuccess ? "Completed" : "Failed",
                Amount = amount,
                CreatedAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = paymentSuccess ? "Pending" : "Payment Failed";
            }

            await _context.SaveChangesAsync();

            return paymentSuccess;
        }
    }
}