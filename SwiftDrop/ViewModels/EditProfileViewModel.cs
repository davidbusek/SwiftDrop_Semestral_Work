using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// Form model for updating a user's basic profile information (name and phone number).
    /// Email and role are intentionally excluded — email is the login identifier
    /// and role changes are an admin-only operation.
    /// </summary>
    public class EditProfileViewModel
    {
        /// <summary>User's first name.</summary>
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>User's last name.</summary>
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>Optional contact phone number.</summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>Read-only email shown on the form (not editable, not submitted).</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Read-only role label shown on the form.</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Customer's saved delivery addresses shown in the Addresses section.</summary>
        public List<Address> SavedAddresses { get; set; } = new();
    }
}
