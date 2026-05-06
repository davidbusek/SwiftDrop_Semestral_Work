using System.ComponentModel.DataAnnotations;

namespace SwiftDrop.ViewModels
{
    /// <summary>View model for the login form.</summary>
    public class LoginViewModel
    {
        /// <summary>User's registered email address.</summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Plain-text password submitted by the user (never persisted).</summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
