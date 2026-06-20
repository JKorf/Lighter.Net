using CryptoExchange.Net.Objects.Errors;

namespace Lighter.Net
{
    internal static class LighterErrors
    {
        public static ErrorMapping Errors { get; } = new ErrorMapping(
            [
                new ErrorInfo(ErrorType.Unauthorized, false, "Invalid credentials", ["21102", "21107", "21109", "21110", "21120"]),

                new ErrorInfo(ErrorType.InsufficientBalance, false, "Insufficient balance", ["21301", "21304", "21305"]),

                new ErrorInfo(ErrorType.InvalidQuantity, false, "Invalid quantity", ["21603", "21624", "21625", "21701", "21706"]),
                new ErrorInfo(ErrorType.InvalidPrice, false, "Invalid price", ["21702"]),

                new ErrorInfo(ErrorType.UnavailableSymbol, false, "Invalid market status", ["21605"]),

                new ErrorInfo(ErrorType.UnknownOrder, false, "Unknown order index", ["21700"]),

                new ErrorInfo(ErrorType.RateLimitRequest, false, "Too many requests", ["23000"]),
                new ErrorInfo(ErrorType.RateLimitConnection, false, "Too many connections", ["23003"]),

                new ErrorInfo(ErrorType.RateLimitOrder, false, "Maximum active account orders reached", ["21717"]),
                new ErrorInfo(ErrorType.RateLimitOrder, false, "Maximum active symbol orders reached", ["21718"]),
                new ErrorInfo(ErrorType.RateLimitOrder, false, "Maximum pending account orders reached", ["21719"]),
                new ErrorInfo(ErrorType.RateLimitOrder, false, "Maximum pending symbol orders reached", ["21720"]),

                new ErrorInfo(ErrorType.DuplicateClientOrderId, false, "Duplicate client order id", ["21728"]),

            ]
            );
    }
}
