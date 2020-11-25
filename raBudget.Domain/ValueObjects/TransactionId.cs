using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(TransactionIdConverter))]
    public class TransactionId : IdValueBase<Guid>
    {
        public TransactionId() : base(Guid.NewGuid())
        {
        }

        public TransactionId(Guid value) : base(value)
        {
        }
    }

    public class TransactionIdConverter : JsonConverter<TransactionId>
    {
        /// <inheritdoc />
        public override TransactionId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new TransactionId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TransactionId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
}