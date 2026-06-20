using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lighter.Net.Converters
{
    internal class JsonRawStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteRawValue(value);
    }
}
