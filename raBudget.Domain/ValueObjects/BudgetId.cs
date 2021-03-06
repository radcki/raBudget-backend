﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    [JsonConverter(typeof(BudgetIdConverter))]
    [TypeConverter(typeof(BudgetIdTypeConverter))]
    public class BudgetId : IdValueBase<Guid>
    {
        public BudgetId() : base(Guid.NewGuid())
        {
        }

        public BudgetId(Guid value) : base(value)
        {
        }
    }

    public class BudgetIdConverter : JsonConverter<BudgetId>
    {
        /// <inheritdoc />
        public override BudgetId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new BudgetId(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BudgetId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
    public class BudgetIdTypeConverter : TypeConverter
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
                return new BudgetId(Guid.ParseExact(stringValue, "N"));
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}