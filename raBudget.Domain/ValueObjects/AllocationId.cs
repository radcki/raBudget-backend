using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [TypeConverter(typeof(AllocationIdTypeConverter))]
    [JsonConverter(typeof(AllocationIdConverter))]
    public record AllocationId(Guid Value)
    {
        public AllocationId() : this(Guid.NewGuid()){}
        public AllocationId(string stringValue) : this(Guid.ParseExact(stringValue, "N")){}
        protected Guid Value { get; init; } = Value;
        public override string ToString() => Value.ToString("N");
    }

    public class AllocationIdConverter : JsonConverter<AllocationId>
    {
        /// <inheritdoc />
        public override AllocationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new AllocationId(reader.GetString());
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, AllocationId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        #endregion
    }

    public class AllocationIdTypeConverter : TypeConverter
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
                return new AllocationId(Guid.ParseExact(stringValue, "N"));
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}