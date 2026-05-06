using System.ComponentModel.DataAnnotations;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// Form model used by the admin to create a new <c>RestaurantManager</c> account.
    /// </summary>
    public class CreateManagerViewModel
    {
        /// <summary>Manager's first name.</summary>
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Manager's last name.</summary>
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>Unique email address used for login.</summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Plain-text password; hashed with BCrypt before storage.</summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>Optional contact phone number.</summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }
}
