using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// Form model used by the admin to create a new restaurant.
    /// Combines restaurant details, a physical address (stored as a new <see cref="Address"/>
    /// record whose <c>UserId</c> is set to <see cref="ManagerId"/>), and the manager assignment.
    /// The address <c>UserId = ManagerId</c> link is how the existing schema associates
    /// a manager with their restaurant.
    /// </summary>
    public class CreateRestaurantViewModel
    {
        // ── Restaurant ───────────────────────────────────────────────────────────

        /// <summary>Display name of the restaurant.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Short description shown on the listing page.</summary>
        public string? Description { get; set; }

        /// <summary>Contact phone number.</summary>
        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        /// <summary>Contact email address.</summary>
        [EmailAddress]
        [MaxLength(255)]
        public string? ContactEmail { get; set; }

        /// <summary>URL of the restaurant's logo image.</summary>
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        /// <summary>Typical food preparation time in minutes.</summary>
        [Range(1, 300)]
        public int? EstimatedPrepTimeMinutes { get; set; }

        /// <summary>Minimum order amount in CZK.</summary>
        [Range(0, 100000)]
        public decimal? MinimumOrderAmount { get; set; }

        // ── Address ──────────────────────────────────────────────────────────────

        /// <summary>Street name and house number of the restaurant's location.</summary>
        [Required(ErrorMessage = "Street is required.")]
        [MaxLength(255)]
        public string Street { get; set; } = string.Empty;

        /// <summary>City of the restaurant's location.</summary>
        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>Postal / ZIP code of the restaurant's location.</summary>
        [Required(ErrorMessage = "ZIP code is required.")]
        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        // ── Manager assignment ───────────────────────────────────────────────────

        /// <summary>
        /// Primary key of the <c>RestaurantManager</c> user to assign to this restaurant.
        /// The created <see cref="Address"/> record will have its <c>UserId</c> set to this value,
        /// which is the schema's convention for linking a restaurant to its manager.
        /// </summary>
        [Required(ErrorMessage = "A manager must be assigned.")]
        public int ManagerId { get; set; }

        /// <summary>
        /// All users with the <c>RestaurantManager</c> role, used to populate the manager drop-down.
        /// Populated by the controller; not submitted as part of the form.
        /// </summary>
        public List<User> AvailableManagers { get; set; } = new();
    }
}
