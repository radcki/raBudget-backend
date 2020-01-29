using System;

namespace raBudget.Domain.BaseTypes
{
    public abstract class IdValueBase<T> : IEquatable<IdValueBase<T>>
    {
        #region Constructors

        protected IdValueBase(T value)
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
}