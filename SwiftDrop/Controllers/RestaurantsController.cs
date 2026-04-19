using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Models;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _restaurantService.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);
            if (restaurant == null) return NotFound();

            var categories = await _restaurantService.GetCategoriesWithMenuItemsAsync(id);

            ViewBag.Categories = categories;
            return View(restaurant);
        }

        public IActionResult Create() => View();

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