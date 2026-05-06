using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Models;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Handles browsing of restaurants and their menus.
    /// </summary>
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        /// <summary>
        /// Initializes a new instance of <see cref="RestaurantsController"/>.
        /// </summary>
        /// <param name="restaurantService">Restaurant data service with optional caching.</param>
        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        /// <summary>Displays a list of all restaurants.</summary>
        public async Task<IActionResult> Index()
        {
            var data = await _restaurantService.GetAllAsync();
            return View(data);
        }

        /// <summary>
        /// Displays the detail page of a restaurant including its menu categories and items.
        /// </summary>
        /// <param name="id">Primary key of the restaurant.</param>
        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null) return NotFound();

            ViewBag.Categories = await _restaurantService.GetCategoriesWithMenuItemsAsync(id);
            return View(restaurant);
        }

        /// <summary>Displays the restaurant creation form.</summary>
        public IActionResult Create() => View();

        /// <summary>
        /// Persists a new restaurant to the database.
        /// </summary>
        /// <param name="restaurant">Restaurant data submitted from the form.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                await _restaurantService.CreateAsync(restaurant);
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }
    }
}
