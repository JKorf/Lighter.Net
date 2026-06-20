using Lighter.Net.Enums;
using System;

namespace Lighter.Net.Objects.Models
{
    /// <summary>
    /// Order request
    /// </summary>
    public record LighterOrderRequest
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Order side
        /// </summary>
        public OrderSide Side { get; set; }
        /// <summary>
        /// Order type
        /// </summary>
        public OrderType OrderType { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Time in force
        /// </summary>
        public TimeInForce TimeInForce { get; set; }
        /// <summary>
        /// Client order index
        /// </summary>
        public long? ClientOrderIndex { get; set; }
        /// <summary>
        /// Reduce only
        /// </summary>
        public bool? ReduceOnly { get; set; }
        /// <summary>
        /// Trigger price
        /// </summary>
        public decimal? TriggerPrice { get; set; }
        /// <summary>
        /// Order expiry
        /// </summary>
        public DateTime? ExpireTime { get; set; }
    }
}
