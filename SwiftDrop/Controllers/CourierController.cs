using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _courierService.GetDashboardDataAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeliveryState(int id)
        {
            await _courierService.AdvanceDeliveryStateAsync(id);
            return RedirectToAction("Index");
        }
    }
}