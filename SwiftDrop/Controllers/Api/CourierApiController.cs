using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // Only for the sake of the Trivial Console Client without JWT setup
    public class CourierApiController : ControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierApiController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            var data = await _courierService.GetDashboardDataAsync();
            return Ok(data);
        }

        [HttpPost("advance-state/{id}")]
        public async Task<IActionResult> AdvanceState(int id)
        {
            await _courierService.AdvanceDeliveryStateAsync(id);
            return Ok(new { message = $"Order {id} state advanced successfully" });
        }
    }
}