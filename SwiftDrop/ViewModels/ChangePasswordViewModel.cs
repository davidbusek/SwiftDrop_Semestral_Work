using System.ComponentModel.DataAnnotations;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// Form model for changing a user's password.
    /// Requires the current password to be verified before the new one is saved.
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>The user's current password, used to verify identity before the change.</summary>
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>The desired new password.</summary>
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>Confirmation of <see cref="NewPassword"/>; must match exactly.</summary>
        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
