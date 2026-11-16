# Lighter.Net examples

Each file is an independent console program. Install `JKorf.Lighter.Net` in a console project and copy one program to `Program.cs`.

| Program | Workflow |
|---|---|
| [01-quickstart.cs](01-quickstart.cs) | Public/account reads and order-submission pattern |
| [02-trading.cs](02-trading.cs) | Leverage, optional margin/order/batch submissions, order queries and cancellation |
| [03-websocket.cs](03-websocket.cs) | Public/private subscriptions and cleanup |
| [04-multi-exchange.cs](04-multi-exchange.cs) | Shared API V2 ticker, symbol catalog and subscription |
| [05-error-handling.cs](05-error-handling.cs) | Results, public-read retries, and Shared symbol support |

Replace credential/index placeholders for intended account operations. The trading program changes leverage when executed, even with its optional order flags disabled. Signed operations require a compatible native signer binary. Read [operational considerations](../../.claude/skills/lighter-net/references/safety.md) first; see [usage snippets](../../.claude/skills/lighter-net/references/usage.md) for API-key-only and L1 credential roles.

`AiExampleCompileTests` compiles the programs without running exchange operations or testing native signing.
