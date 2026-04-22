using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.ViewModels;
using System.Collections.Generic;

namespace SwiftDrop.Services
{
    public interface ICourierService
    {
        Task<CourierDashboardViewModel> GetDashboardDataAsync();
        Task AdvanceDeliveryStateAsync(int orderId);
    }

    public class CourierService : ICourierService
    {
        private readonly SwiftDropDbContext _context;

        public CourierService(SwiftDropDbContext context)
        {
            _context = context;
        }

        public async Task<CourierDashboardViewModel> GetDashboardDataAsync()
        {
            var today = System.DateTime.Today;

            var todaysDeliveryFees = await _context.Orders
                .Where(o => o.Status == "Delivered" && o.DeliveredAt >= today)
                .SumAsync(o => (decimal?)o.DeliveryFee) ?? 0m;
            
            var courierEarnings = todaysDeliveryFees * 0.8m;

            var availableOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Suborders)
                    .ThenInclude(s => s.Orderitems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Suborders)
                    .ThenInclude(s => s.Restaurant)
                    .ThenInclude(r => r.Address)
                .Where(o => o.Status == "CourierAssigned" || o.Status == "Delivering")
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

            var markers = new List<MapMarkerDto>();

            foreach(var order in availableOrders)
            {
                // Pickup points
                foreach(var sub in order.Suborders)
                {
                    if(sub.Restaurant != null && sub.Restaurant.Address != null && 
                       sub.Restaurant.Address.Latitude.HasValue && sub.Restaurant.Address.Longitude.HasValue)
                    {
                        markers.Add(new MapMarkerDto {
                            OrderId = order.Id,
                            Lat = sub.Restaurant.Address.Latitude.Value,
                            Lng = sub.Restaurant.Address.Longitude.Value,
                            Title = $"[Pickup #{order.Id}] {sub.Restaurant.Name}",
                            Type = "Pickup"
                        });
                    }
                }

                // Destination point
                if (order.Address != null && order.Address.Latitude.HasValue && order.Address.Longitude.HasValue)
                {
                    markers.Add(new MapMarkerDto {
                        OrderId = order.Id,
                        Lat = order.Address.Latitude.Value,
                        Lng = order.Address.Longitude.Value,
                        Title = $"[Dropoff #{order.Id}] {order.Address.Street}",
                        Type = "Delivery"
                    });
                }
            }

            return new CourierDashboardViewModel
            {
                ActiveJobs = availableOrders.Count,
                CurrentEarnings = courierEarnings,
                AvailableOrders = availableOrders,
                MapMarkers = markers
            };
        }

        public async Task AdvanceDeliveryStateAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                if (order.Status == "CourierAssigned")
                {
                    order.Status = "Delivering";
                }
                else if (order.Status == "Delivering")
                {
                    order.Status = "Delivered";
                    order.DeliveredAt = System.DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}