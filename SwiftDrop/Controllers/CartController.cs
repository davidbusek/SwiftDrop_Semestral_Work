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
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}