using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Restaurant
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? LogoUrl { get; set; }

    public decimal? AverageRating { get; set; }

    public int? ReviewCount { get; set; }

    public int? EstimatedPrepTimeMinutes { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsAcceptingOrders { get; set; }

    public int AddressId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Openinghour> Openinghours { get; set; } = new List<Openinghour>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Suborder> Suborders { get; set; } = new List<Suborder>();
}
