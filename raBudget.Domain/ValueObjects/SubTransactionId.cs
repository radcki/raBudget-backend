using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(SubTransactionIdConverter))]
    [TypeConverter(typeof(SubTransactionIdTypeConverter))]
    public record SubTransactionId(Guid Value)
    {
        public SubTransactionId() : this(Guid.NewGuid()){}
        public SubTransactionId(string stringValue) : this(Guid.ParseExact(stringValue, "N")){}
        protected Guid Value { get; init; } = Value;
        public override string ToString() => Value.ToString("N");
    }

    public class SubTransactionIdConverter : JsonConverter<SubTransactionId>
    {
        /// <inheritdoc />
        public override SubTransactionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new SubTransactionId(reader.GetString());
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, SubTransactionId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
        #endregion
    }
    public class SubTransactionIdTypeConverter : TypeConverter
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
                return new SubTransactionId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}