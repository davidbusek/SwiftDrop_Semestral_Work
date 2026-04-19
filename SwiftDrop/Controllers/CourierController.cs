using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        public IActionResult Index()
        {
            var model = new CourierDashboardViewModel
            {
                ActiveJobs = 3,
                CurrentEarnings = 750.00m
            };

            return View(model);
        }
    }
}