using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [TypeConverter(typeof(TransactionTemplateIdTypeConverter))]
    [JsonConverter(typeof(TransactionTemplateIdConverter))]
    public class TransactionTemplateId : IdValueBase<Guid>
    {
        public TransactionTemplateId() : base(Guid.NewGuid())
        {
        }

        public TransactionTemplateId(Guid value) : base(value)
        {
        }
    }

    public class TransactionTemplateIdConverter : JsonConverter<TransactionTemplateId>
    {
        /// <inheritdoc />
        public override TransactionTemplateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new TransactionTemplateId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TransactionTemplateId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
    public class TransactionTemplateIdTypeConverter : TypeConverter
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
                return new TransactionTemplateId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}