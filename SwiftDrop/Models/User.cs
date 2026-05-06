using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

/// <summary>
/// Represents a registered user of the SwiftDrop platform.
/// The <see cref="Role"/> field controls access to role-restricted controllers
/// (<c>Admin</c>, <c>RestaurantManager</c>, <c>Courier</c>, <c>Customer</c>).
/// </summary>
public partial class User
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>User's first name.</summary>
    public string FirstName { get; set; } = null!;

    /// <summary>User's last name.</summary>
    public string LastName { get; set; } = null!;

    /// <summary>Unique email address used for login.</summary>
    public string Email { get; set; } = null!;

    /// <summary>Optional phone number.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>BCrypt hash of the user's password.</summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>Role string stored as a claim: <c>Admin</c>, <c>RestaurantManager</c>, <c>Courier</c>, or <c>Customer</c>.</summary>
    public string Role { get; set; } = null!;

    /// <summary>Timestamp when the account was created.</summary>
    public DateTime? RegisteredAt { get; set; }

    /// <summary>Indicates whether the account is active.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Delivery addresses saved by this user.</summary>
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    /// <summary>Orders placed by this user.</summary>
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>Reviews written by this user.</summary>
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
