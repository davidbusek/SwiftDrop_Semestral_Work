using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AddressId { get; set; }

    public string Status { get; set; } = null!;

    public decimal ItemPrice { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Suborder> Suborders { get; set; } = new List<Suborder>();

    public virtual User User { get; set; } = null!;
}
