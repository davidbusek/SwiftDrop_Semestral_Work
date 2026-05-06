using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Initializes a new instance of <see cref="CartController"/>.
        /// </summary>
        /// <param name="cartService">Session-backed cart service.</param>
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>Displays the cart summary with item list, subtotal, delivery fee and total.</summary>
        [HttpGet]
        public IActionResult Index()
        {
            var cartItems = _cartService.GetCart();
            var deliveryFee = _cartService.GetTotalDeliveryPrice();
            var subtotal = cartItems.Sum(i => i.Price * i.Quantity);

            return View(new CartViewModel
            {
                CartItems = cartItems,
                DeliveryFee = deliveryFee,
                Subtotal = subtotal,
                Total = subtotal + deliveryFee
            });
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
        /// Processes the checkout: validates the delivery address, creates the order,
        /// runs mock payment and clears the cart.
        /// Requires the user to be authenticated.
        /// </summary>
        /// <param name="street">Delivery street.</param>
        /// <param name="city">Delivery city.</param>
        /// <param name="zipCode">Delivery ZIP code.</param>
        /// <param name="orderService">Order service injected per-request.</param>
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Checkout(
            string street, string city, string zipCode,
            [FromServices] IOrderService orderService)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Account");

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
