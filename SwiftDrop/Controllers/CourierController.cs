using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Dashboard for couriers.
    /// Accessible only to users with the <c>Courier</c> role.
    /// </summary>
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly ICourierService _courierService;

        /// <summary>
        /// Initializes a new instance of <see cref="CourierController"/>.
        /// </summary>
        /// <param name="courierService">Courier service for deliveries and map data.</param>
        public CourierController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        /// <summary>Displays the courier dashboard with available orders and an optimized map route.</summary>
        public async Task<IActionResult> Index()
        {
            var model = await _courierService.GetDashboardDataAsync();
            return View(model);
        }

        /// <summary>
        /// Advances the delivery state of the order identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        [HttpPost]
        public async Task<IActionResult> UpdateDeliveryState(int id)
        {
            await _courierService.AdvanceDeliveryStateAsync(id);
            return RedirectToAction("Index");
        }
    }
}
