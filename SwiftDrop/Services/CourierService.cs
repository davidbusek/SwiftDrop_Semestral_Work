using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Services.OrderStates;
using SwiftDrop.ViewModels;
using System.Collections.Generic;

namespace SwiftDrop.Services
{
    /// <summary>
    /// Provides courier-facing operations: dashboard data with available deliveries,
    /// map markers with an optimized pickup route, and order state advancement.
    /// </summary>
    public interface ICourierService
    {
        /// <summary>
        /// Returns all orders in <c>CourierAssigned</c> or <c>Delivering</c> state,
        /// along with map markers sorted by the optimal pickup route.
        /// </summary>
        Task<CourierDashboardViewModel> GetDashboardDataAsync();

        /// <summary>
        /// Advances the delivery state of the order identified by <paramref name="orderId"/>
        /// using the State Pattern (<see cref="OrderStateFactory"/>).
        /// </summary>
        /// <param name="orderId">Primary key of the order to advance.</param>
        Task AdvanceDeliveryStateAsync(int orderId);
    }

    /// <summary>EF Core implementation of <see cref="ICourierService"/>.</summary>
    public class CourierService : ICourierService
    {
        private readonly SwiftDropDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="CourierService"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        public CourierService(SwiftDropDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
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

            foreach (var order in availableOrders)
            {
                var orderPickups = new List<MapMarkerDto>();
                MapMarkerDto? orderDropoff = null;

                foreach (var sub in order.Suborders)
                {
                    if (sub.Restaurant != null && sub.Restaurant.Address != null &&
                        sub.Restaurant.Address.Latitude.HasValue && sub.Restaurant.Address.Longitude.HasValue)
                    {
                        orderPickups.Add(new MapMarkerDto
                        {
                            OrderId = order.Id,
                            Lat = sub.Restaurant.Address.Latitude.Value,
                            Lng = sub.Restaurant.Address.Longitude.Value,
                            Title = $"[Pickup #{order.Id}] {sub.Restaurant.Name}",
                            Type = "Pickup"
                        });
                    }
                }

                if (order.Address != null && order.Address.Latitude.HasValue && order.Address.Longitude.HasValue)
                {
                    orderDropoff = new MapMarkerDto
                    {
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

        /// <summary>
        /// Returns <paramref name="pickups"/> reordered to minimize total travel distance
        /// before reaching <paramref name="dropoff"/> (brute-force TSP for small pickup counts).
        /// </summary>
        /// <param name="pickups">Restaurant pickup points for a single order.</param>
        /// <param name="dropoff">Customer delivery destination, or <c>null</c> if unavailable.</param>
        private List<MapMarkerDto> OptimizeRoute(List<MapMarkerDto> pickups, MapMarkerDto? dropoff)
        {
            if (!pickups.Any())
            {
                if (dropoff != null) { dropoff.RouteOrder = 1; return new List<MapMarkerDto> { dropoff }; }
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

            var bestPath = new List<MapMarkerDto>();
            double minDistance = double.MaxValue;

            foreach (var path in GetPermutations(pickups, pickups.Count))
            {
                double currentDist = 0;
                for (int i = 0; i < path.Count - 1; i++)
                    currentDist += CalculateDistance(path[i].Lat, path[i].Lng, path[i + 1].Lat, path[i + 1].Lng);

                if (dropoff != null)
                    currentDist += CalculateDistance(path.Last().Lat, path.Last().Lng, dropoff.Lat, dropoff.Lng);

                if (currentDist < minDistance) { minDistance = currentDist; bestPath = path.ToList(); }
            }

            for (int i = 0; i < bestPath.Count; i++)
                bestPath[i].RouteOrder = i + 1;

            var result = new List<MapMarkerDto>(bestPath);
            if (dropoff != null) { dropoff.RouteOrder = bestPath.Count + 1; result.Add(dropoff); }
            return result;
        }

        /// <summary>
        /// Calculates the great-circle distance between two GPS coordinates using the Haversine formula.
        /// </summary>
        /// <returns>Distance in metres.</returns>
        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371e3;
            var radLat1 = Math.PI * (double)lat1 / 180;
            var radLat2 = Math.PI * (double)lat2 / 180;
            var theta = Math.PI * (double)(lat2 - lat1) / 180;
            var lambda = Math.PI * (double)(lon2 - lon1) / 180;

            var a = Math.Sin(theta / 2) * Math.Sin(theta / 2) +
                    Math.Cos(radLat1) * Math.Cos(radLat2) *
                    Math.Sin(lambda / 2) * Math.Sin(lambda / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        /// <summary>Generates all permutations of <paramref name="list"/> with the given <paramref name="length"/>.</summary>
        private IEnumerable<List<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new List<T> { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new[] { t2 }).ToList());
        }

        /// <inheritdoc/>
        public async Task AdvanceDeliveryStateAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                var state = OrderStateFactory.GetState(order.Status);
                if (state.CanAdvance)
                    state.Advance(order);
                await _context.SaveChangesAsync();
            }
        }

    }
}
