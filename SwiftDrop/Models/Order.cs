using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

/// <summary>
/// Represents a customer order that may span multiple restaurants.
/// Each restaurant's portion is tracked as a separate <see cref="SubOrder"/>.
/// Status transitions are managed by the State Pattern via
/// <see cref="SwiftDrop.Services.OrderStates.OrderStateFactory"/>.
/// </summary>
public partial class Order
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key of the customer who placed the order.</summary>
    public int UserId { get; set; }

    /// <summary>Foreign key of the delivery address for this order.</summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Current status string. Valid values: <c>Pending</c>, <c>Paid</c>,
    /// <c>PickupsInProgress</c>, <c>CourierAssigned</c>, <c>Delivering</c>,
    /// <c>Delivered</c>, <c>Canceled</c>.
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>Sum of all item unit prices multiplied by their quantities, in CZK.</summary>
    public decimal ItemPrice { get; set; }

    /// <summary>Delivery fee calculated by <see cref="SwiftDrop.Services.IDeliveryCostStrategy"/>.</summary>
    public decimal DeliveryFee { get; set; }

    /// <summary>Total charged to the customer: <see cref="ItemPrice"/> + <see cref="DeliveryFee"/>.</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Timestamp when the order was created.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Timestamp set when the order transitions to <c>Delivered</c>.</summary>
    public DateTime? DeliveredAt { get; set; }

    /// <summary>
    /// Foreign key of the courier assigned to this order.
    /// Set when the courier first advances the state from <c>CourierAssigned</c>.
    /// <c>null</c> means the order is not yet claimed by any courier.
    /// Requires <c>ALTER TABLE Orders ADD COLUMN CourierId INT NULL;</c> on existing databases.
    /// </summary>
    public int? CourierId { get; set; }

    /// <summary>Delivery address navigation property.</summary>
    public virtual Address Address { get; set; } = null!;

    /// <summary>The courier assigned to deliver this order (nullable).</summary>
    public virtual User? Courier { get; set; }

    /// <summary>Payment records associated with this order.</summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>Per-restaurant sub-orders that make up this order.</summary>
    public virtual ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();

    /// <summary>Customer who placed the order.</summary>
    public virtual User User { get; set; } = null!;
}
