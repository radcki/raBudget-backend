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