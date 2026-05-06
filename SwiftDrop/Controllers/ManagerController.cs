using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Dashboard for restaurant managers.
    /// Accessible only to users with the <c>RestaurantManager</c> role.
    /// </summary>
    [Authorize(Roles = "RestaurantManager")]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        /// <summary>
        /// Initializes a new instance of <see cref="ManagerController"/>.
        /// </summary>
        /// <param name="managerService">Manager service for order and menu-item operations.</param>
        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        /// <summary>Displays the manager dashboard with pending orders and menu items.</summary>
        public async Task<IActionResult> Index()
        {
            var model = await _managerService.GetDashboardDataAsync();
            return View(model);
        }

        /// <summary>
        /// Advances the order identified by <paramref name="id"/> to its next state.
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        [HttpPost]
        public async Task<IActionResult> AdvanceOrderState(int id)
        {
            await _managerService.AdvanceOrderStateAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes the menu item identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the menu item to delete.</param>
        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            await _managerService.DeleteMenuItemAsync(id);
            return RedirectToAction("Index");
        }
    }
}
