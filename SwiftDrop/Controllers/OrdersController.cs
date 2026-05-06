using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
