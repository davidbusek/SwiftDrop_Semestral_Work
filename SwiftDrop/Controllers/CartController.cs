using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Models;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
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
    }
}