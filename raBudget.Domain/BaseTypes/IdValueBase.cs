using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using raBudget.Domain.Models;

namespace raBudget.Domain.BaseTypes
{
    [JsonConverter(typeof(IdValueBaseConverter))]
    public class IdValueBase<T> : IEquatable<IdValueBase<T>>
    {
        #region Constructors

        public IdValueBase(T value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public T Value { get; }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IdValueBase<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }


        public bool Equals(IdValueBase<T> other)
        {
            return Equals(Value, other.Value);
        }

        public static bool operator ==(IdValueBase<T> obj1, IdValueBase<T> obj2)
        {
            if (Equals(obj1, null))
            {
                if (Equals(obj2, null))
                {
                    return true;
                }

                return false;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(IdValueBase<T> x, IdValueBase<T> y)
        {
            return !(x == y);
        }

        #endregion
    }

    public class IdValueBaseConverter : JsonConverter<IdValueBase<Guid>>
    {
        /// <inheritdoc />
        public override IdValueBase<Guid>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new IdValueBase<Guid>(Guid.ParseExact(reader.GetString(), "N"));
        }

        #region Overrides of JsonConverter<IdValueBase<Guid>>

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IdValueBase<Guid> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value.ToString("N"));
        }
        #endregion
    }
}