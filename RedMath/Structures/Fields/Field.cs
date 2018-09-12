using System;
using System.Diagnostics;

namespace RedMath.Structures
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    public abstract class Field<T> : IEquatable<T> where T : Field<T>, new()
    {
        protected abstract T _zero { get; }
        protected abstract T _one { get; }
        public abstract T AdditiveInverse { get; }
        public abstract T MultiplicativeInverse { get; }
        public abstract T Add(T other);
        public abstract T Multiply(T other);

        private static readonly T _instance = new T();
        public static T Zero => _instance._zero;
        public static T One => _instance._one;

        public T Subtract(T other) => Add(other.AdditiveInverse);
        public T Divide(T other) => Multiply(other.MultiplicativeInverse);

        public static T Add(T left, T right) => left.Add(right);
        public static T Multiply(T left, T right) => left.Multiply(right);
        public static T Subtract(T left, T right) => left.Subtract(right);
        public static T Divide(T left, T right) => left.Divide(right);

        public static T operator +(Field<T> left, T right) => left.Add(right);
        public static T operator *(Field<T> left, T right) => left.Multiply(right);
        public static T operator -(Field<T> left, T right) => left.Subtract(right);
        public static T operator /(Field<T> left, T right) => left.Divide(right);

        public static T operator -(Field<T> element) => element.AdditiveInverse;


        public abstract T Clone();
        public abstract bool Equals(T other);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((T)obj);
        }

        public static bool operator ==(Field<T> left, Field<T> right) => left.Equals(right);
        public static bool operator !=(Field<T> left, Field<T> right) => !left.Equals(right);
    }

#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

}
