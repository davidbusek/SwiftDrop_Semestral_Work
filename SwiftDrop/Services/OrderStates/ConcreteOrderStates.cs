using SwiftDrop.Models;

namespace SwiftDrop.Services.OrderStates
{
    /// <summary>
    /// Order has been placed but payment has not yet been confirmed.
    /// Transitions to <c>PickupsInProgress</c> when a manager accepts the order.
    /// </summary>
    public class PendingState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "Pending";
        /// <inheritdoc/>
        public bool CanAdvance => true;
        /// <inheritdoc/>
        public void Advance(Order order) => order.Status = "PickupsInProgress";
    }

    /// <summary>
    /// Payment was successfully collected. Functionally identical to <see cref="PendingState"/>
    /// from the manager's perspective — both advance to <c>PickupsInProgress</c>.
    /// </summary>
    public class PaidState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "Paid";
        /// <inheritdoc/>
        public bool CanAdvance => true;
        /// <inheritdoc/>
        public void Advance(Order order) => order.Status = "PickupsInProgress";
    }

    /// <summary>
    /// Restaurant(s) are preparing the items. Manager advances to <c>CourierAssigned</c>
    /// once all sub-orders are ready for pickup.
    /// </summary>
    public class PickupsInProgressState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "PickupsInProgress";
        /// <inheritdoc/>
        public bool CanAdvance => true;
        /// <inheritdoc/>
        public void Advance(Order order) => order.Status = "CourierAssigned";
    }

    /// <summary>
    /// A courier has been assigned and is on their way to pick up the order.
    /// Transitions to <c>Delivering</c> when the courier confirms pickup.
    /// </summary>
    public class CourierAssignedState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "CourierAssigned";
        /// <inheritdoc/>
        public bool CanAdvance => true;
        /// <inheritdoc/>
        public void Advance(Order order) => order.Status = "Delivering";
    }

    /// <summary>
    /// The courier is actively delivering the order to the customer.
    /// Transitions to <c>Delivered</c> and stamps <see cref="Order.DeliveredAt"/> with the current time.
    /// </summary>
    public class DeliveringState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "Delivering";
        /// <inheritdoc/>
        public bool CanAdvance => true;
        /// <inheritdoc/>
        public void Advance(Order order)
        {
            order.Status = "Delivered";
            order.DeliveredAt = DateTime.Now;
        }
    }

    /// <summary>
    /// Terminal state — order was successfully delivered to the customer.
    /// No further transitions are possible.
    /// </summary>
    public class DeliveredState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "Delivered";
        /// <inheritdoc/>
        public bool CanAdvance => false;
        /// <inheritdoc/>
        public void Advance(Order order) { }
    }

    /// <summary>
    /// Terminal state — order was canceled (e.g. payment failure).
    /// No further transitions are possible.
    /// </summary>
    public class CanceledState : IOrderState
    {
        /// <inheritdoc/>
        public string StatusName => "Canceled";
        /// <inheritdoc/>
        public bool CanAdvance => false;
        /// <inheritdoc/>
        public void Advance(Order order) { }
    }
}
