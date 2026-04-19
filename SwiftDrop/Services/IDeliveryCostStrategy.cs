using System.Collections.Generic;
using SwiftDrop.Models;

namespace SwiftDrop.Services
{
    public interface IDeliveryCostStrategy
    {
        decimal CalculateDeliveryCost(IEnumerable<CartItem> items);
    }
}