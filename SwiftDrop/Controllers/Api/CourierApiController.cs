using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers.Api
{
    /// <summary>
    /// REST API exposing courier operations for external clients.
    /// Allows the <c>SwiftDrop.Client</c> console application to read dashboard
    /// data and advance order states without a browser session.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
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
        /// <returns>200 OK with <see cref="SwiftDrop.ViewModels.CourierDashboardViewModel"/> payload.</returns>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            var data = await _courierService.GetDashboardDataAsync();
            return Ok(data);
        }

        /// <summary>
        /// Advances the delivery state of the order identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the order to advance.</param>
        /// <returns>200 OK with a confirmation message.</returns>
        [HttpPost("advance-state/{id}")]
        public async Task<IActionResult> AdvanceState(int id)
        {
            await _courierService.AdvanceDeliveryStateAsync(id);
            return Ok(new { message = $"Order {id} state advanced successfully" });
        }
    }
}
