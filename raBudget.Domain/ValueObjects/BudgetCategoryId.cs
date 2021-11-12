using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [TypeConverter(typeof(BudgetCategoryIdTypeConverter))]
    [JsonConverter(typeof(BudgetCategoryIdConverter))]
    public record BudgetCategoryId(Guid Value)
    {
        public BudgetCategoryId() : this(Guid.NewGuid()){}
        public BudgetCategoryId(string stringValue) : this(Guid.ParseExact(stringValue, "N")){}

        protected Guid Value { get; init; } = Value;
        public override string ToString() => Value.ToString("N");
    }

    public class BudgetCategoryIdConverter : JsonConverter<BudgetCategoryId>
    {
        /// <inheritdoc />
        public override BudgetCategoryId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return string.IsNullOrWhiteSpace(value)
                       ? null
                       : new BudgetCategoryId(value);
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetCategoryId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        #endregion
    }

    public class BudgetCategoryIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            Console.WriteLine("CanConvertFrom " + sourceType.ToString());
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Console.WriteLine("BudgetCategoryIdTypeConverter ConvertFrom");
            if (value is string stringValue)
            {
                return new BudgetCategoryId(Guid.ParseExact(stringValue, "N"));
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}