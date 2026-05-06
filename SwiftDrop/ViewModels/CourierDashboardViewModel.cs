using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// View model for the courier dashboard.
    /// Bundles active delivery statistics with the ordered list of map markers
    /// produced by the route optimization algorithm in
    /// <see cref="SwiftDrop.Services.CourierService"/>.
    /// </summary>
    public class CourierDashboardViewModel
    {
        /// <summary>Number of orders currently in <c>CourierAssigned</c> or <c>Delivering</c> state.</summary>
        public int ActiveJobs { get; set; }

        /// <summary>Courier's earnings for today (80 % of today's delivered order fees), in CZK.</summary>
        public decimal CurrentEarnings { get; set; }

        /// <summary>Orders available for pickup or currently being delivered.</summary>
        public List<Order> AvailableOrders { get; set; } = new();

        /// <summary>
        /// Map markers for all active orders, sorted by the optimized pickup route.
        /// Each marker's <see cref="MapMarkerDto.RouteOrder"/> indicates the suggested stop sequence.
        /// </summary>
        public List<MapMarkerDto> MapMarkers { get; set; } = new();
    }
}
