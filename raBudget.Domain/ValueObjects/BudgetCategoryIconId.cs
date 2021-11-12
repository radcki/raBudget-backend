using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(BudgetCategoryIconIdConverter))]
    [TypeConverter(typeof(BudgetCategoryIconIdTypeConverter))]
    public record BudgetCategoryIconId (Guid Value)
    {
        public BudgetCategoryIconId() : this(Guid.NewGuid()){}
        public BudgetCategoryIconId(string stringValue) : this(Guid.ParseExact(stringValue, "N")){}
        protected Guid Value { get; init; } = Value;
        public override string ToString() => Value.ToString("N");
}

    public class BudgetCategoryIconIdConverter : JsonConverter<BudgetCategoryIconId>
    {
        /// <inheritdoc />
        public override BudgetCategoryIconId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new BudgetCategoryIconId(reader.GetString());
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetCategoryIconId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
        #endregion
    }

    public class BudgetCategoryIconIdTypeConverter : TypeConverter
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
                return new BudgetCategoryIconId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}