using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(BudgetIdConverter))]
    public class BudgetId : IdValueBase<Guid>
    {
        public BudgetId() : base(Guid.NewGuid())
        {
        }

        public BudgetId(Guid value) : base(value)
        {
        }
    }

    public class BudgetIdConverter : JsonConverter<BudgetId>
    {
        /// <inheritdoc />
        public override BudgetId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new BudgetId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
}