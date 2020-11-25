using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(BudgetedAmountIdConverter))]
    public class BudgetedAmountId : IdValueBase<Guid>
    {
        public BudgetedAmountId() : base(Guid.NewGuid()) { }
        public BudgetedAmountId(Guid value) : base(value) { }
    }

    public class BudgetedAmountIdConverter : JsonConverter<BudgetedAmountId>
    {
        /// <inheritdoc />
        public override BudgetedAmountId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new BudgetedAmountId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetedAmountId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
}