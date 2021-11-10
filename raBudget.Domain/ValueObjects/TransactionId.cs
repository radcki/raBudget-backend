using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [TypeConverter(typeof(TransactionIdTypeConverter))]
    [JsonConverter(typeof(TransactionIdConverter))]
    public class TransactionId : IdValueBase<Guid>
    {
        public TransactionId() : base(Guid.NewGuid())
        {
        }

        public TransactionId(Guid value) : base(value)
        {
        }
    }

    public class TransactionIdConverter : JsonConverter<TransactionId>
    {
        /// <inheritdoc />
        public override TransactionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new TransactionId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TransactionId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
    public class TransactionIdTypeConverter : TypeConverter
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
                return new TransactionId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}