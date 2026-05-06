using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Dashboard for restaurant managers.
    /// Accessible only to users with the <c>RestaurantManager</c> role.
    /// All data is scoped to the restaurants associated with the logged-in manager
    /// via the <c>Address.UserId</c> convention.
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

        /// <summary>
        /// Returns the primary key of the currently authenticated manager from the <c>UserId</c> claim.
        /// </summary>
        private int CurrentManagerId =>
            int.Parse(User.FindFirstValue("UserId")!);

        /// <summary>Displays the manager dashboard with pending orders and menu items scoped to this manager's restaurants.</summary>
        public async Task<IActionResult> Index()
        {
            var model = await _managerService.GetDashboardDataAsync(CurrentManagerId);
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

        // ── Categories ───────────────────────────────────────────────────────────

        /// <summary>
        /// Displays the form for creating a new menu category.
        /// The restaurant drop-down is limited to restaurants owned by the current manager.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            var model = new CreateCategoryViewModel
            {
                AvailableRestaurants = await _managerService.GetManagerRestaurantsAsync(CurrentManagerId)
            };
            return View(model);
        }

        /// <summary>
        /// Processes the create-category form.
        /// Redirects back to the dashboard on success; re-renders the form on validation failure.
        /// </summary>
        /// <param name="model">Validated form data.</param>
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRestaurants = await _managerService.GetManagerRestaurantsAsync(CurrentManagerId);
                return View(model);
            }

            await _managerService.CreateCategoryAsync(model);
            TempData["Success"] = $"Category \"{model.Name}\" created successfully.";
            return RedirectToAction("Index");
        }

        // ── Menu Items ───────────────────────────────────────────────────────────

        /// <summary>
        /// Displays the form for creating a new menu item.
        /// The category drop-down is limited to categories belonging to the current manager's restaurants.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateMenuItem()
        {
            var model = new CreateMenuItemViewModel
            {
                Categories = await _managerService.GetManagerCategoriesAsync(CurrentManagerId)
            };
            return View(model);
        }

        /// <summary>
        /// Processes the create-menu-item form.
        /// Redirects back to the dashboard on success; re-renders the form on validation failure.
        /// </summary>
        /// <param name="model">Validated form data.</param>
        [HttpPost]
        public async Task<IActionResult> CreateMenuItem(CreateMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _managerService.GetManagerCategoriesAsync(CurrentManagerId);
                return View(model);
            }

            await _managerService.CreateMenuItemAsync(model);
            TempData["Success"] = $"Menu item \"{model.Name}\" created successfully.";
            return RedirectToAction("Index");
        }
    }
}
