using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.Services;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Dashboard for platform administrators.
    /// Accessible only to users with the <c>Admin</c> role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        /// <summary>
        /// Initializes a new instance of <see cref="AdminController"/>.
        /// </summary>
        /// <param name="adminService">Admin service providing statistics and user management.</param>
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>Displays the admin dashboard with platform statistics and recent users.</summary>
        public async Task<IActionResult> Index()
        {
            var model = await _adminService.GetDashboardDataAsync();
            return View(model);
        }

        /// <summary>
        /// Permanently deletes the user identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Primary key of the user to delete.</param>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _adminService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}
