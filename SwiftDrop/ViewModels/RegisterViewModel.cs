using System.ComponentModel.DataAnnotations;

namespace SwiftDrop.ViewModels
{
    /// <summary>View model for the new account registration form.</summary>
    public class RegisterViewModel
    {
        /// <summary>User's first name.</summary>
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>User's last name.</summary>
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>Email address that will be used as the login identifier.</summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>Plain-text password (minimum 6 characters); hashed with BCrypt before storage.</summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
