using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(SubTransactionIdConverter))]
    public class SubTransactionId : IdValueBase<Guid>
    {
        public SubTransactionId() : base(Guid.NewGuid())
        {
        }

        public SubTransactionId(Guid value) : base(value)
        {
        }
    }

    public class SubTransactionIdConverter : JsonConverter<SubTransactionId>
    {
        /// <inheritdoc />
        public override SubTransactionId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new SubTransactionId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, SubTransactionId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
}