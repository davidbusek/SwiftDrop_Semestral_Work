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
                var orderPickups = new List<MapMarkerDto>();
                MapMarkerDto orderDropoff = null;

                // Pickup points
                foreach(var sub in order.Suborders)
                {
                    if(sub.Restaurant != null && sub.Restaurant.Address != null && 
                       sub.Restaurant.Address.Latitude.HasValue && sub.Restaurant.Address.Longitude.HasValue)
                    {
                        orderPickups.Add(new MapMarkerDto {
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
                    orderDropoff = new MapMarkerDto {
                        OrderId = order.Id,
                        Lat = order.Address.Latitude.Value,
                        Lng = order.Address.Longitude.Value,
                        Title = $"[Dropoff #{order.Id}] {order.Address.Street}",
                        Type = "Delivery"
                    };
                }

                var optimizedRoute = OptimizeRoute(orderPickups, orderDropoff);
                markers.AddRange(optimizedRoute);
            }

            return new CourierDashboardViewModel
            {
                ActiveJobs = availableOrders.Count,
                CurrentEarnings = courierEarnings,
                AvailableOrders = availableOrders,
                MapMarkers = markers
            };
        }

        private List<MapMarkerDto> OptimizeRoute(List<MapMarkerDto> pickups, MapMarkerDto dropoff)
        {
            if (!pickups.Any()) 
            {
                if (dropoff != null) 
                {
                    dropoff.RouteOrder = 1;
                    return new List<MapMarkerDto> { dropoff };
                }
                return new List<MapMarkerDto>();
            }

            if (pickups.Count == 1)
            {
                pickups[0].RouteOrder = 1;
                if (dropoff != null) dropoff.RouteOrder = 2;
                var res = new List<MapMarkerDto> { pickups[0] };
                if (dropoff != null) res.Add(dropoff);
                return res;
            }

            // Route Optimization Algorithm (Traveling Salesperson Problem specific approach)
            // Finds the optimal sequence of Pickups to minimize travel distance before reaching the Dropoff.
            var bestPath = new List<MapMarkerDto>();
            double minDistance = double.MaxValue;

            var permutations = GetPermutations(pickups, pickups.Count);
            foreach (var path in permutations)
            {
                double currentDist = 0;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    currentDist += CalculateDistance(path[i].Lat, path[i].Lng, path[i+1].Lat, path[i+1].Lng);
                }
                if (dropoff != null)
                {
                    currentDist += CalculateDistance(path.Last().Lat, path.Last().Lng, dropoff.Lat, dropoff.Lng);
                }

                if (currentDist < minDistance)
                {
                    minDistance = currentDist;
                    bestPath = path.ToList();
                }
            }

            for (int i = 0; i < bestPath.Count; i++)
            {
                bestPath[i].RouteOrder = i + 1;
            }

            var result = new List<MapMarkerDto>(bestPath);
            if (dropoff != null)
            {
                dropoff.RouteOrder = bestPath.Count + 1;
                result.Add(dropoff);
            }

            return result;
        }

        // Haversine formula to calculate the distance between two GPS coordinates
        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var r = 6371e3; // Earth Radius in meters
            var radLat1 = Math.PI * (double)lat1 / 180;
            var radLat2 = Math.PI * (double)lat2 / 180;
            var theta = Math.PI * (double)(lat2 - lat1) / 180;
            var lambda = Math.PI * (double)(lon2 - lon1) / 180;

            var a = Math.Sin(theta / 2) * Math.Sin(theta / 2) +
                    Math.Cos(radLat1) * Math.Cos(radLat2) *
                    Math.Sin(lambda / 2) * Math.Sin(lambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return r * c;
        }

        private IEnumerable<List<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new List<T> { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }).ToList());
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