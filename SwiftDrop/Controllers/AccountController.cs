using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SwiftDrop.Data;
using SwiftDrop.Models;
using SwiftDrop.ViewModels;

namespace SwiftDrop.Controllers
{
    /// <summary>
    /// Handles user registration, login and logout using cookie-based authentication.
    /// Passwords are hashed with BCrypt before storage.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly SwiftDropDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="AccountController"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        public AccountController(SwiftDropDbContext context)
        {
            _context = context;
        }

        /// <summary>Displays the login form.</summary>
        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        /// <summary>
        /// Authenticates the user. On success, issues a cookie and redirects to the home page.
        /// </summary>
        /// <param name="model">Login credentials.</param>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
            return View(model);
        }

        /// <summary>Displays the registration form.</summary>
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        /// <summary>
        /// Creates a new customer account. Rejects duplicate email addresses.
        /// </summary>
        /// <param name="model">Registration data.</param>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Customer",
                RegisteredAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }

        /// <summary>Signs the user out and redirects to the home page.</summary>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
