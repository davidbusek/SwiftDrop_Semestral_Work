using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Orderitem
{
    public int Id { get; set; }

    public int SubOrderId { get; set; }

    public int MenuItemId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string? ItemNotes { get; set; }

    public virtual Menuitem MenuItem { get; set; } = null!;

    public virtual Suborder SubOrder { get; set; } = null!;
}
