using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Models;
using SwiftDrop.Services;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cartItems = _cartService.GetCart();
            var deliveryFee = _cartService.GetTotalDeliveryPrice();
            var subtotal = cartItems.Sum(i => i.Price * i.Quantity);

            var viewModel = new CartViewModel
            {
                CartItems = cartItems,
                DeliveryFee = deliveryFee,
                Subtotal = subtotal,
                Total = subtotal + deliveryFee
            };

            return View(viewModel);
        }

        [HttpPost("api/cart/add")]
        public IActionResult Add([FromBody] CartItem item)
        {
            if (item == null)
            {
                return BadRequest("Invalid cart item.");
            }

            if (item.Quantity <= 0)
            {
                item.Quantity = 1; // Default to 1 if not specified
            }

            _cartService.AddToCart(item);

            var cart = _cartService.GetCart();
            var totalCount = cart.Sum(i => i.Quantity);
            var deliveryFee = _cartService.GetTotalDeliveryPrice();

            return Ok(new
            {
                cartCount = totalCount,
                deliveryFee = deliveryFee
            });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, int quantity)
        {
            _cartService.UpdateQuantity(menuItemId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int menuItemId)
        {
            _cartService.RemoveItem(menuItemId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Checkout([FromServices] IOrderService orderService)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _cartService.GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            var deliveryFee = _cartService.GetTotalDeliveryPrice();

            // Vytvo?ení objednávky
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Index");
            }

            var order = await orderService.ProcessCheckoutAsync(userEmail, cart, deliveryFee);

            // Mock Platba
            bool isSuccess = await orderService.MockPaymentProcessAsync(order.Id, order.TotalPrice);

            _cartService.ClearCart();

            if (isSuccess)
            {
                TempData["SuccessMessage"] = $"Payment successful! Order #{order.Id} confirmed and is now pending.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Payment link failed for Order #{order.Id}. Transaction denied.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}