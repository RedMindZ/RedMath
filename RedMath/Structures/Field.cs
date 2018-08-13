using System;
using System.Diagnostics;

namespace RedMath.Structures
{
    #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    #pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    public abstract class Field<T> : IEquatable<T> where T : Field<T>
    {
        public abstract T Zero { get; }
        public abstract T One { get; }



        public abstract T AdditiveInverse { get; }
        public abstract T MultiplicativeInverse { get; }



        public abstract T Add(T other);
        public abstract T Multiply(T other);

        public T Subtract(T other)
        {
            return Add(other.AdditiveInverse);
        }

        public T Divide(T other)
        {
            return Multiply(other.MultiplicativeInverse);
        }


        public static T Add(T left, T right)
        {
            return left.Add(right);
        }

        public static T Multiply(T left, T right)
        {
            return left.Multiply(right);
        }

        public static T Subtract(T left, T right)
        {
            return left.Subtract(right);
        }

        public static T Divide(T left, T right)
        {
            return left.Divide(right);
        }


        public static T operator +(Field<T> left, T right)
        {
            return left.Add(right);
        }

        public static T operator *(Field<T> left, T right)
        {
            return left.Multiply(right);
        }

        public static T operator -(Field<T> left, T right)
        {
            return left.Subtract(right);
        }

        public static T operator /(Field<T> left, T right)
        {
            return left.Divide(right);
        }

        public static T operator -(Field<T> element)
        {
            return element.AdditiveInverse;
        }



        public abstract T Clone();

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((T)obj);
        }

        public abstract bool Equals(T other);

        public static bool operator ==(Field<T> left, Field<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Field<T> left, Field<T> right)
        {
            return !left.Equals(right);
        }
    }

    #pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    #pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

}
