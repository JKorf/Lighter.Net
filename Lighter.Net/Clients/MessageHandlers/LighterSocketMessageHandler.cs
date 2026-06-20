using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using System.Text.Json;
using CryptoExchange.Net.Converters.SystemTextJson.MessageHandlers;

namespace Lighter.Net.Clients.MessageHandlers
{
    internal class LighterSocketMessageHandler : JsonSocketMessageHandler
    {
        public override JsonSerializerOptions Options { get; } = LighterExchange._serializerContext;

        public LighterSocketMessageHandler()
        {
        }

        protected override MessageTypeDefinition[] TypeEvaluators { get; } = [

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("id"),
                ],
                ForceIfFound = true,
                TypeIdentifierCallback = x => x.FieldValue("id")!,
            },

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("code") { Depth = 2 },
                    new PropertyFieldReference("message") { Depth = 2 },
                ],
                StaticIdentifier = "error",
            },

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("type"),
                    new PropertyFieldReference("channel"),
                ],
                TypeIdentifierCallback = x => x.FieldValue("type") + x.FieldValue("channel")!,
            },

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("type").WithEqualConstraint("pong"),
                ],
                ForceIfFound = true,
                StaticIdentifier = "pong",
            },

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("type").WithEqualConstraint("connected"),
                ],
                ForceIfFound = true,
                StaticIdentifier = "connected",
            }
        ];
    }
}
