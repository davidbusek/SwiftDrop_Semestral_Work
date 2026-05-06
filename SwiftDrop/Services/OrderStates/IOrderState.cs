using SwiftDrop.Models;

namespace SwiftDrop.Services.OrderStates
{
    /// <summary>
    /// Defines the contract for a single state in the order lifecycle (State Pattern).
    /// Each concrete state encapsulates the transition logic for that specific status,
    /// eliminating scattered if/else chains in service classes.
    /// </summary>
    public interface IOrderState
    {
        /// <summary>Gets the string status identifier stored in the database for this state.</summary>
        string StatusName { get; }

        /// <summary>
        /// Gets a value indicating whether the order can be advanced from this state.
        /// Terminal states (<see cref="DeliveredState"/>, <see cref="CanceledState"/>) return <c>false</c>.
        /// </summary>
        bool CanAdvance { get; }

        /// <summary>
        /// Transitions the given order to the next state by mutating <see cref="Order.Status"/>
        /// and any other relevant fields (e.g. <see cref="Order.DeliveredAt"/>).
        /// Has no effect when <see cref="CanAdvance"/> is <c>false</c>.
        /// </summary>
        /// <param name="order">The order whose state should be advanced.</param>
        void Advance(Order order);
    }
}
