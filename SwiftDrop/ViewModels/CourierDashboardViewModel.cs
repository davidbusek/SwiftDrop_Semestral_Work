using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    public class CourierDashboardViewModel
    {
        public int ActiveJobs { get; set; }
        public decimal CurrentEarnings { get; set; }
        public List<Order> AvailableOrders { get; set; } = new();
        public List<MapMarkerDto> MapMarkers { get; set; } = new();
    }
}