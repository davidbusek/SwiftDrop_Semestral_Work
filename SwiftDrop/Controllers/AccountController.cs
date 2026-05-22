using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
                PhoneNumber = model.PhoneNumber,
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

        // ── Profile ──────────────────────────────────────────────────────────────

        /// <summary>Loads delivery addresses for the given user, used across profile actions.</summary>
        private async Task<List<Address>> GetDeliveryAddressesAsync(int userId) =>
            await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDeliveryAddress)
                .OrderByDescending(a => a.Id)
                .ToListAsync();

        /// <summary>
        /// Displays the authenticated user's profile page with an edit form,
        /// a password change form and a saved delivery addresses section.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction(nameof(Login));

            ViewBag.ChangePassword = new ChangePasswordViewModel();
            return View(new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = user.Role,
                SavedAddresses = await GetDeliveryAddressesAsync(user.Id)
            });
        }

        /// <summary>
        /// Updates the authenticated user's first name, last name and phone number.
        /// Re-issues the authentication cookie so the navbar name updates immediately.
        /// </summary>
        /// <param name="model">Validated profile form data.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(EditProfileViewModel model)
        {
            // Remove server-side validation for read-only display fields
            ModelState.Remove(nameof(EditProfileViewModel.Email));
            ModelState.Remove(nameof(EditProfileViewModel.Role));

            if (!ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var u = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                model.Email = u?.Email ?? string.Empty;
                model.Role = u?.Role ?? string.Empty;
                if (u != null) model.SavedAddresses = await GetDeliveryAddressesAsync(u.Id);
                ViewBag.ChangePassword = new ChangePasswordViewModel();
                return View("Profile", model);
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToAction(nameof(Login));

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            await _context.SaveChangesAsync();

            // Re-issue cookie so the navbar picks up the new name immediately
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            TempData["ProfileSuccess"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// Changes the authenticated user's password after verifying the current one.
        /// </summary>
        /// <param name="model">Validated password change form data.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var u = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                ViewBag.ChangePassword = model;
                return View("Profile", new EditProfileViewModel
                {
                    FirstName = u?.FirstName ?? string.Empty,
                    LastName = u?.LastName ?? string.Empty,
                    PhoneNumber = u?.PhoneNumber,
                    Email = u?.Email ?? string.Empty,
                    Role = u?.Role ?? string.Empty,
                    SavedAddresses = u != null ? await GetDeliveryAddressesAsync(u.Id) : new()
                });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToAction(nameof(Login));

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Current password is incorrect.");
                ViewBag.ChangePassword = model;
                return View("Profile", new EditProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role = user.Role,
                    SavedAddresses = await GetDeliveryAddressesAsync(user.Id)
                });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["PasswordSuccess"] = "Password changed successfully.";
            return RedirectToAction(nameof(Profile));
        }

        // ── Delivery Addresses ────────────────────────────────────────────────────

        /// <summary>Saves a new delivery address for the authenticated user.</summary>
        /// <param name="street">Street name and house number.</param>
        /// <param name="city">City name.</param>
        /// <param name="zipCode">Postal code.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAddress(string street, string city, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(zipCode))
            {
                TempData["AddressError"] = "All address fields are required.";
                return RedirectToAction(nameof(Profile));
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction(nameof(Login));

            var duplicate = await _context.Addresses.AnyAsync(a =>
                a.UserId == user.Id && a.IsDeliveryAddress &&
                a.Street == street.Trim() && a.City == city.Trim() && a.ZipCode == zipCode.Trim());

            if (!duplicate)
            {
                _context.Addresses.Add(new Address
                {
                    UserId = user.Id,
                    Street = street.Trim(),
                    City = city.Trim(),
                    ZipCode = zipCode.Trim(),
                    IsDeliveryAddress = true
                });
                await _context.SaveChangesAsync();
                TempData["AddressSuccess"] = "Address added.";
            }
            else
            {
                TempData["AddressError"] = "This address is already saved.";
            }

            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// Updates an existing delivery address. Resets geocoordinates so they are
        /// refreshed by Nominatim on the next checkout.
        /// </summary>
        /// <param name="addressId">Primary key of the address to update.</param>
        /// <param name="street">New street.</param>
        /// <param name="city">New city.</param>
        /// <param name="zipCode">New postal code.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(int addressId, string street, string city, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(zipCode))
            {
                TempData["AddressError"] = "All address fields are required.";
                return RedirectToAction(nameof(Profile));
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction(nameof(Login));

            var addr = await _context.Addresses.FirstOrDefaultAsync(a =>
                a.Id == addressId && a.UserId == user.Id && a.IsDeliveryAddress);

            if (addr == null)
            {
                TempData["AddressError"] = "Address not found.";
                return RedirectToAction(nameof(Profile));
            }

            addr.Street = street.Trim();
            addr.City = city.Trim();
            addr.ZipCode = zipCode.Trim();
            addr.Latitude = null;
            addr.Longitude = null;
            await _context.SaveChangesAsync();

            TempData["AddressSuccess"] = "Address updated.";
            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// Deletes a saved delivery address. Refuses deletion if the address is
        /// referenced by an existing order (FK constraint).
        /// </summary>
        /// <param name="addressId">Primary key of the address to delete.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction(nameof(Login));

            var addr = await _context.Addresses.FirstOrDefaultAsync(a =>
                a.Id == addressId && a.UserId == user.Id && a.IsDeliveryAddress);

            if (addr == null)
            {
                TempData["AddressError"] = "Address not found.";
                return RedirectToAction(nameof(Profile));
            }

            bool usedByOrder = await _context.Orders.AnyAsync(o => o.AddressId == addressId);
            if (usedByOrder)
            {
                TempData["AddressError"] = "This address cannot be deleted because it is linked to an existing order.";
                return RedirectToAction(nameof(Profile));
            }

            _context.Addresses.Remove(addr);
            await _context.SaveChangesAsync();

            TempData["AddressSuccess"] = "Address deleted.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
