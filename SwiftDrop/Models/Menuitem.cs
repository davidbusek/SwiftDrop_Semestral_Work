using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Menuitem
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public string? Allergens { get; set; }

    public string? WeightOrVolume { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();
}
