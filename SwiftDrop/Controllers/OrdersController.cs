using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetUserOrdersByEmailAsync(userEmail);

            var model = new MyOrdersViewModel
            {
                Orders = orders
            };

            return View(model);
        }
    }
}