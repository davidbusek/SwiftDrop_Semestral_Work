using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    [Authorize(Roles = "RestaurantManager")]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _managerService.GetDashboardDataAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AdvanceOrderState(int id)
        {
            await _managerService.AdvanceOrderStateAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            await _managerService.DeleteMenuItemAsync(id);
            return RedirectToAction("Index");
        }
    }
}