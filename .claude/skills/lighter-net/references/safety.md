# Lighter operational considerations

## Wallet and API signing roles

`LighterCredentials` pairs Ethereum wallet information with account index, API key index, and a separate private API key. Use `EthKey.FromPublicAddress` when the operation only needs API-key signing; L1-signed operations such as integrator approval require `EthKey.FromPrivateKey`. Do not substitute the wallet secret for the API secret or treat generating an API key pair locally as registering/activating it on the account. Keep private keys in secret storage; ensure account/key indexes identify the intended account.

Signer-backed requests require compatible native assets. A missing or incompatible binary is a deployment problem, not a transient exchange rejection. `LibraryPath` selects an explicit file; match the binary to the process platform/architecture.

## Nonces and transaction outcomes

Let the library populate nonces for normal workflows. When several clients or processes use one account/API key, coordinate signing and nonce ownership rather than manually retrying the same transaction or resetting a nonce after an error. Retain transaction hashes and client order indexes for reconciliation. Shared placement returns a null order ID on success, so supply and retain a numeric client order ID when the application needs external correlation.

Successful transaction submission does not prove the order is open, fully filled, or finalized. Query transaction/order state or consume order updates. A limit order can fill immediately, and cancellation can race with fills. Cancel only the order index belonging to the intended order; do not cancel the first open order as generic cleanup. For grouped or batch requests, inspect returned transaction/error details and reconcile every intended order; an outer success does not establish that all intended fills happened.

## Perpetual exposure and account changes

`SetLeverageAsync` and `UpdateMarginAsync` alter real account risk; account-tier and signer transactions can also change account behavior. For a close, read current position, derive the opposite side and current base quantity, and use `reduceOnly` where appropriate. Do not import another exchange's hedge-mode flags or contract-count sizing. Transfers and withdrawals move funds; confirm destination/account, asset and amount for the requested operation.

## Integrator fee

REST and socket options default `IntegratorFeePercentage` to `0.01m` (0.01%, one basis point). Approval is a signed account change; check the configured fee policy and required L1 signature before using `ApproveIntegratorAsync`. `0m` or null disables the optional integrator fee. Configure this explicitly when required by the application rather than silently changing fees while implementing unrelated behavior.

## Uncertain submissions and retries

A timeout or connection loss can leave a signed request accepted but unconfirmed. `Error.IsTransient` alone does not justify repeating orders, cancellations, transfers, withdrawals, margin/leverage changes, or approvals. Reconcile transaction hash, order/client indexes, trades, balances, or account history before deciding whether to resubmit, allowing for propagation delays. Bounded backoff is suitable for transient public reads; keep a generic read-retry helper away from account changes.
