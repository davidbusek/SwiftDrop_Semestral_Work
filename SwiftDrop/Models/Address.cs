using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

/// <summary>
/// Represents a physical address used for restaurant locations and delivery destinations.
/// GPS coordinates (<see cref="Latitude"/>, <see cref="Longitude"/>) are populated
/// by the Nominatim geocoding call during checkout.
/// </summary>
public partial class Address
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Street name and house number.</summary>
    public string Street { get; set; } = null!;

    /// <summary>City name.</summary>
    public string City { get; set; } = null!;

    /// <summary>Postal / ZIP code.</summary>
    public string ZipCode { get; set; } = null!;

    /// <summary>WGS-84 latitude; <c>null</c> when geocoding was not performed or failed.</summary>
    public decimal? Latitude { get; set; }

    /// <summary>WGS-84 longitude; <c>null</c> when geocoding was not performed or failed.</summary>
    public decimal? Longitude { get; set; }

    /// <summary>Foreign key of the user who owns this address.</summary>
    public int UserId { get; set; }

    /// <summary>
    /// <c>true</c> for customer delivery addresses shown on checkout.
    /// <c>false</c> for restaurant pickup addresses, which share this table
    /// but must never appear in the customer address picker.
    /// </summary>
    public bool IsDeliveryAddress { get; set; }

    /// <summary>Orders delivered to this address.</summary>
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>Restaurants located at this address.</summary>
    public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

    /// <summary>Owner of this address.</summary>
    public virtual User User { get; set; } = null!;
}
