using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Suborder
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int RestaurantId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? EstimatedReadyTime { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
