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
    public class AllocationId : IdValueBase<Guid>
    {
        public AllocationId() : base(Guid.NewGuid())
        {
        }

        public AllocationId(Guid value) : base(value)
        {
        }
    }

    public class AllocationIdConverter : JsonConverter<AllocationId>
    {
        /// <inheritdoc />
        public override AllocationId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new AllocationId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, AllocationId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
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