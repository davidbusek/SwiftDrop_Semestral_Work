using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftDrop.Data;
using SwiftDrop.Models;
using SwiftDrop.Services;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Manages the customer's shopping cart and triggers the checkout flow.
    /// Cart state is persisted in the HTTP session via <see cref="ICartService"/>.
    /// </summary>
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly SwiftDropDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="CartController"/>.
        /// </summary>
        /// <param name="cartService">Session-backed cart service.</param>
        /// <param name="context">Database context used to load saved addresses.</param>
        public CartController(ICartService cartService, SwiftDropDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        /// <summary>
        /// Displays the cart summary with item list, subtotal, delivery fee and total.
        /// When the user is authenticated their saved addresses are included so the view
        /// can render address radio buttons on the checkout form.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartItems = _cartService.GetCart();
            var deliveryFee = _cartService.GetTotalDeliveryPrice();
            var subtotal = cartItems.Sum(i => i.Price * i.Quantity);

            var vm = new CartViewModel
            {
                CartItems = cartItems,
                DeliveryFee = deliveryFee,
                Subtotal = subtotal,
                Total = subtotal + deliveryFee
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    vm.SavedAddresses = await _context.Addresses
                        .Where(a => a.User.Email == email && a.IsDeliveryAddress)
                        .OrderByDescending(a => a.Id)
                        .ToListAsync();
                }
            }

            return View(vm);
        }

        /// <summary>
        /// Adds an item to the cart (called via JSON from the restaurant detail page).
        /// Returns the updated total item count and delivery fee.
        /// </summary>
        /// <param name="item">Item to add.</param>
        [HttpPost("api/cart/add")]
        public IActionResult Add([FromBody] CartItem item)
        {
            if (item == null) return BadRequest("Invalid cart item.");
            if (item.Quantity <= 0) item.Quantity = 1;

            _cartService.AddToCart(item);

            var cart = _cartService.GetCart();
            return Ok(new
            {
                cartCount = cart.Sum(i => i.Quantity),
                deliveryFee = _cartService.GetTotalDeliveryPrice()
            });
        }

        /// <summary>Updates the quantity of a specific menu item in the cart.</summary>
        /// <param name="menuItemId">ID of the item to update.</param>
        /// <param name="quantity">New quantity; 0 removes the item.</param>
        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, int quantity)
        {
            _cartService.UpdateQuantity(menuItemId, quantity);
            return RedirectToAction("Index");
        }

        /// <summary>Removes a specific item from the cart.</summary>
        /// <param name="menuItemId">ID of the item to remove.</param>
        [HttpPost]
        public IActionResult Remove(int menuItemId)
        {
            _cartService.RemoveItem(menuItemId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Processes the checkout: resolves the delivery address (saved or newly entered),
        /// creates the order, runs mock payment and clears the cart on success.
        /// Requires the user to be authenticated.
        /// </summary>
        /// <param name="savedAddressId">
        /// ID of a previously saved address to use. When &gt; 0 the street/city/zipCode
        /// parameters are ignored and the stored address is used instead.
        /// </param>
        /// <param name="street">Delivery street — required when <paramref name="savedAddressId"/> is 0.</param>
        /// <param name="city">Delivery city — required when <paramref name="savedAddressId"/> is 0.</param>
        /// <param name="zipCode">Delivery ZIP code — required when <paramref name="savedAddressId"/> is 0.</param>
        /// <param name="orderService">Order service injected per-request.</param>
        [HttpPost]
        public async Task<IActionResult> Checkout(
            int savedAddressId,
            string? street, string? city, string? zipCode,
            [FromServices] IOrderService orderService)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (savedAddressId > 0)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var addr = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == savedAddressId && a.User.Email == email && a.IsDeliveryAddress);

                if (addr == null)
                {
                    TempData["ErrorMessage"] = "Selected address not found.";
                    return RedirectToAction("Index");
                }

                street = addr.Street;
                city = addr.City;
                zipCode = addr.ZipCode;
            }

            if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(zipCode))
            {
                TempData["ErrorMessage"] = "Please fill in your delivery address before placing the order.";
                return RedirectToAction("Index");
            }

            var cart = _cartService.GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            var deliveryFee = _cartService.GetTotalDeliveryPrice();
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Index");

            var order = await orderService.ProcessCheckoutAsync(userEmail, cart, deliveryFee,
                street.Trim(), city.Trim(), zipCode.Trim());

            bool isSuccess = await orderService.MockPaymentProcessAsync(order.Id, order.TotalPrice);

            if (isSuccess)
            {
                _cartService.ClearCart();
                TempData["SuccessMessage"] = $"Payment successful! Order #{order.Id} confirmed and is now pending.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Payment failed for Order #{order.Id}. Transaction denied. Your cart has been kept.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>Clears all items from the cart.</summary>
        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}
