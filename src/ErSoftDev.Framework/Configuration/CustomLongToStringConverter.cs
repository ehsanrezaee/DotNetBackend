using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Text.Json;

namespace ErSoftDev.Framework.Configuration
{
    public class CustomLongToStringConverter : System.Text.Json.Serialization.JsonConverter<long>
    {
        public CustomLongToStringConverter()
        {

        }
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence
                    ? reader.ValueSequence.ToArray()
                    : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out long number, out int bytesConsumed)
                    && span.Length == bytesConsumed)
                {
                    return number;
                }

                if (long.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }

            return reader.GetInt64();
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }
}
