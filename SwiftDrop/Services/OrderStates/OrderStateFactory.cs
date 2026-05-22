using SwiftDrop.Models;

namespace SwiftDrop.Services.OrderStates
{
    /// <summary>
    /// Creates the correct <see cref="IOrderState"/> instance for a given status string.
    /// Acts as the entry point to the State Pattern — callers never instantiate state
    /// objects directly; they ask the factory and work through the <see cref="IOrderState"/> interface.
    /// </summary>
    public static class OrderStateFactory
    {
        /// <summary>
        /// Returns the <see cref="IOrderState"/> that corresponds to <paramref name="status"/>.
        /// </summary>
        /// <param name="status">
        /// The order status string as stored in the database
        /// (e.g. <c>"Pending"</c>, <c>"Delivered"</c>).
        /// </param>
        /// <returns>The matching state object.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="status"/> does not match any known state.
        /// </exception>
        public static IOrderState GetState(string status) => status switch
        {
            OrderStatus.Pending            => new PendingState(),
            OrderStatus.Paid               => new PaidState(),
            OrderStatus.PickupsInProgress  => new PickupsInProgressState(),
            OrderStatus.CourierAssigned    => new CourierAssignedState(),
            OrderStatus.Delivering         => new DeliveringState(),
            OrderStatus.Delivered          => new DeliveredState(),
            OrderStatus.Canceled           => new CanceledState(),
            _                              => throw new InvalidOperationException($"Unknown order status: {status}")
        };
    }
}
