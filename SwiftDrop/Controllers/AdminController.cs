using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Services;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Dashboard for platform administrators.
    /// Accessible only to users with the <c>Admin</c> role.
    /// Supports managing users, orders, restaurants and menu items.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly SwiftDropDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="AdminController"/>.
        /// </summary>
        /// <param name="adminService">Admin service providing statistics and management operations.</param>
        /// <param name="context">Database context used to populate form drop-downs.</param>
        public AdminController(IAdminService adminService, SwiftDropDbContext context)
        {
            _adminService = adminService;
            _context = context;
        }

        /// <summary>Displays the admin dashboard with platform statistics, users, orders, restaurants and menu items.</summary>
        public async Task<IActionResult> Index()
        {
            var model = await _adminService.GetDashboardDataAsync();
            return View(model);
        }

        /// <summary>
        /// Permanently deletes the user identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the user to delete.</param>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _adminService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }

        // ── Managers ────────────────────────────────────────────────────────────

        /// <summary>Displays the form for creating a new restaurant manager account.</summary>
        [HttpGet]
        public IActionResult CreateManager() => View(new CreateManagerViewModel());

        /// <summary>
        /// Processes the create-manager form.
        /// Redirects back to the dashboard on success; returns the form with an error on duplicate email.
        /// </summary>
        /// <param name="model">Validated form data.</param>
        [HttpPost]
        public async Task<IActionResult> CreateManager(CreateManagerViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var created = await _adminService.CreateManagerAsync(model);
            if (!created)
            {
                ModelState.AddModelError("Email", "This email address is already registered.");
                return View(model);
            }

            TempData["Success"] = $"Manager account for {model.FirstName} {model.LastName} created successfully.";
            return RedirectToAction("Index");
        }

        // ── Menu Items ───────────────────────────────────────────────────────────

        /// <summary>
        /// Displays the form for creating a new menu item.
        /// Populates the category drop-down with all categories grouped by restaurant.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateMenuItem()
        {
            var model = new CreateMenuItemViewModel
            {
                Categories = await _context.Categories
                    .Include(c => c.Restaurant)
                    .OrderBy(c => c.Restaurant.Name).ThenBy(c => c.Name)
                    .ToListAsync()
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
                model.Categories = await _context.Categories
                    .Include(c => c.Restaurant)
                    .OrderBy(c => c.Restaurant.Name).ThenBy(c => c.Name)
                    .ToListAsync();
                return View(model);
            }

            await _adminService.CreateMenuItemAsync(model);
            TempData["Success"] = $"Menu item \"{model.Name}\" created successfully.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Permanently deletes the menu item identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the menu item to delete.</param>
        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            await _adminService.DeleteMenuItemAsync(id);
            return RedirectToAction("Index");
        }

        // ── Restaurants ──────────────────────────────────────────────────────────

        /// <summary>
        /// Permanently deletes the restaurant identified by <paramref name="id"/>
        /// along with all its categories and menu items.
        /// </summary>
        /// <param name="id">Primary key of the restaurant to delete.</param>
        [HttpPost]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            await _adminService.DeleteRestaurantAsync(id);
            return RedirectToAction("Index");
        }

        // ── Categories ───────────────────────────────────────────────────────────

        /// <summary>
        /// Displays the form for creating a new menu category.
        /// Populates the restaurant drop-down with all restaurants.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            var model = new CreateCategoryViewModel
            {
                AvailableRestaurants = await _context.Restaurants
                    .OrderBy(r => r.Name).ToListAsync()
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
                model.AvailableRestaurants = await _context.Restaurants
                    .OrderBy(r => r.Name).ToListAsync();
                return View(model);
            }

            await _adminService.CreateCategoryAsync(model);
            TempData["Success"] = $"Category \"{model.Name}\" created successfully.";
            return RedirectToAction("Index");
        }

        // ── Restaurants (create) ─────────────────────────────────────────────────

        /// <summary>
        /// Displays the form for creating a new restaurant.
        /// Populates the manager drop-down with all <c>RestaurantManager</c> users.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateRestaurant()
        {
            var model = new CreateRestaurantViewModel
            {
                AvailableManagers = await _context.Users
                    .Where(u => u.Role == "RestaurantManager")
                    .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                    .ToListAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Processes the create-restaurant form.
        /// Redirects back to the dashboard on success; re-renders the form on validation failure.
        /// </summary>
        /// <param name="model">Validated form data.</param>
        [HttpPost]
        public async Task<IActionResult> CreateRestaurant(CreateRestaurantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableManagers = await _context.Users
                    .Where(u => u.Role == "RestaurantManager")
                    .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                    .ToListAsync();
                return View(model);
            }

            await _adminService.CreateRestaurantAsync(model);
            TempData["Success"] = $"Restaurant \"{model.Name}\" created successfully.";
            return RedirectToAction("Index");
        }

        // ── Orders ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Advances the order identified by <paramref name="id"/> to its next state
        /// using the State Pattern.
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        [HttpPost]
        public async Task<IActionResult> AdvanceOrderState(int id)
        {
            await _adminService.AdvanceOrderStateAsync(id);
            return RedirectToAction("Index");
        }
    }
}
