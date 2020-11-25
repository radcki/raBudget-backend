using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(BudgetCategoryIconIdConverter))]
    public class BudgetCategoryIconId : IdValueBase<Guid>
    {
        public BudgetCategoryIconId(Guid value) : base(value)
        {
        }

        public BudgetCategoryIconId() : base(Guid.NewGuid())
        {
        }
    }

    public class BudgetCategoryIconIdConverter : JsonConverter<BudgetCategoryIconId>
    {
        /// <inheritdoc />
        public override BudgetCategoryIconId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new BudgetCategoryIconId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetCategoryIconId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
}