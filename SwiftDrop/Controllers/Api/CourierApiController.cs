using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers.Api
{
    /// <summary>
    /// REST API exposing courier operations for external clients (e.g. the SwiftDrop.Client console app).
    /// Requires a valid <c>X-Api-Key</c> header matching <c>ApiKeys:CourierApi</c> in configuration.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class CourierApiController : ControllerBase
    {
        private readonly ICourierService _courierService;

        /// <summary>
        /// Initializes a new instance of <see cref="CourierApiController"/>.
        /// </summary>
        /// <param name="courierService">Courier service shared with the MVC controller.</param>
        public CourierApiController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        /// <summary>
        /// Returns the current courier dashboard data as JSON, including active orders
        /// and optimized map markers.
        /// </summary>
        /// <param name="courierId">Primary key of the courier requesting the data.</param>
        /// <returns>200 OK with <see cref="SwiftDrop.ViewModels.CourierDashboardViewModel"/> payload.</returns>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData([FromQuery] int courierId)
        {
            var data = await _courierService.GetDashboardDataAsync(courierId);
            return Ok(data);
        }

        /// <summary>
        /// Advances the delivery state of the order identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        /// <param name="courierId">Primary key of the courier performing the action.</param>
        /// <returns>200 OK with a confirmation message.</returns>
        [HttpPost("advance-state/{id}")]
        public async Task<IActionResult> AdvanceState(int id, [FromQuery] int courierId)
        {
            await _courierService.AdvanceDeliveryStateAsync(id, courierId);
            return Ok(new { message = $"Order {id} state advanced successfully" });
        }
    }
}
