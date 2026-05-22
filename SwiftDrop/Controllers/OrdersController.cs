using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Models;
using SwiftDrop.Services;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Displays the authenticated customer's order history.
    /// </summary>
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initializes a new instance of <see cref="OrdersController"/>.
        /// </summary>
        /// <param name="orderService">Service for retrieving order data.</param>
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>Displays live tracking for a single order owned by the current user.</summary>
        /// <param name="id">Primary key of the order to track.</param>
        public async Task<IActionResult> Track(int id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderForTrackingAsync(id, userEmail);
            if (order == null) return NotFound();

            var steps = BuildSteps(order.Status);
            return View(new OrderTrackingViewModel { Order = order, Steps = steps });
        }

        private static readonly (string Status, string Icon, string Label)[] StepDefinitions =
        {
            (OrderStatus.Pending,           "receipt",          "Order Placed"),
            (OrderStatus.Paid,              "credit-card",      "Payment Confirmed"),
            (OrderStatus.PickupsInProgress, "bag-check",        "Preparing"),
            (OrderStatus.CourierAssigned,   "person-walking",   "Courier Assigned"),
            (OrderStatus.Delivering,        "bicycle",          "Out for Delivery"),
            (OrderStatus.Delivered,         "house-check",      "Delivered"),
        };

        private static List<TrackingStep> BuildSteps(string currentStatus)
        {
            if (currentStatus == OrderStatus.Canceled)
                return new List<TrackingStep>();

            var currentIndex = System.Array.FindIndex(StepDefinitions, s => s.Status == currentStatus);
            var result = new List<TrackingStep>();

            for (int i = 0; i < StepDefinitions.Length; i++)
            {
                var (_, icon, label) = StepDefinitions[i];
                result.Add(new TrackingStep
                {
                    Icon = icon,
                    Label = label,
                    IsCompleted = i < currentIndex,
                    IsCurrent = i == currentIndex,
                });
            }
            return result;
        }

        /// <summary>Displays a list of all orders placed by the currently logged-in user.</summary>
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Claims
                .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetUserOrdersByEmailAsync(userEmail);
            return View(new MyOrdersViewModel { Orders = orders });
        }
    }
}
