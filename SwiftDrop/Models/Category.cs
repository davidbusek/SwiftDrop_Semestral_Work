using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Category
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public int? DisplayOrder { get; set; }

    public virtual ICollection<Menuitem> Menuitems { get; set; } = new List<Menuitem>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
