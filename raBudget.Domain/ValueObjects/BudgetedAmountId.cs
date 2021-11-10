using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [TypeConverter(typeof(BudgetedAmountIdTypeConverter))]
    [JsonConverter(typeof(BudgetedAmountIdConverter))]
    public class BudgetedAmountId : IdValueBase<Guid>
    {
        public BudgetedAmountId() : base(Guid.NewGuid()) { }
        public BudgetedAmountId(Guid value) : base(value) { }
    }

    public class BudgetedAmountIdConverter : JsonConverter<BudgetedAmountId>
    {
        /// <inheritdoc />
        public override BudgetedAmountId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

    public class BudgetedAmountIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return new BudgetedAmountId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}