using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

/// <summary>
/// Represents a restaurant partner on the SwiftDrop platform.
/// A restaurant has a menu organized into <see cref="Category"/> objects
/// and is linked to a physical <see cref="Address"/> used for courier pickup routing.
/// </summary>
public partial class Restaurant
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the restaurant.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Short description shown on the listing page.</summary>
    public string? Description { get; set; }

    /// <summary>Contact phone number.</summary>
    public string? ContactPhone { get; set; }

    /// <summary>Contact email address.</summary>
    public string? ContactEmail { get; set; }

    /// <summary>URL of the restaurant's logo image.</summary>
    public string? LogoUrl { get; set; }

    /// <summary>Cached average review rating (0–5).</summary>
    public decimal? AverageRating { get; set; }

    /// <summary>Total number of reviews received.</summary>
    public int? ReviewCount { get; set; }

    /// <summary>Typical food preparation time in minutes.</summary>
    public int? EstimatedPrepTimeMinutes { get; set; }

    /// <summary>Minimum order amount in CZK.</summary>
    public decimal? MinimumOrderAmount { get; set; }

    /// <summary>Indicates whether the restaurant is visible in the listing.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Indicates whether the restaurant is currently accepting new orders.</summary>
    public bool? IsAcceptingOrders { get; set; }

    /// <summary>Foreign key of the restaurant's physical address.</summary>
    public int AddressId { get; set; }

    /// <summary>Physical address used for courier pickup.</summary>
    public virtual Address Address { get; set; } = null!;

    /// <summary>Menu categories containing the restaurant's menu items.</summary>
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    /// <summary>Opening hours for each day of the week.</summary>
    public virtual ICollection<Openinghour> Openinghours { get; set; } = new List<Openinghour>();

    /// <summary>Customer reviews for this restaurant.</summary>
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>Sub-orders associated with deliveries from this restaurant.</summary>
    public virtual ICollection<Suborder> Suborders { get; set; } = new List<Suborder>();
}
