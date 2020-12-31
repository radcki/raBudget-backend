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
    public class BudgetCategoryId : IdValueBase<Guid>
    {
        public BudgetCategoryId() : base(Guid.NewGuid()) { }
        public BudgetCategoryId(Guid value) : base(value) { }
    }

    public class BudgetCategoryIdConverter : JsonConverter<BudgetCategoryId>
    {
        /// <inheritdoc />
        public override BudgetCategoryId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new BudgetCategoryId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetCategoryId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }

    public class BudgetCategoryIdTypeConverter : TypeConverter
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
                return new BudgetCategoryId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}