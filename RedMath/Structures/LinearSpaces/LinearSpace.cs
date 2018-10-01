using RedMath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

    public abstract class LinearSpace<T, F> : IAddable<T>, ISubtractable<T>, IEquatable<T> where T : LinearSpace<T, F> where F : Field<F>, new()
    {
        protected abstract T Zero { get; }
        protected abstract F UnitScalar { get; }


        public abstract T AdditiveInverse { get; }
        public abstract T Add(T other);
        public abstract T Multiply(F scalar);

        public T Subtract(T other) => Add(other.AdditiveInverse);
        public T Divide(F scalar) => Multiply(scalar.MultiplicativeInverse);

        public static T Add(T left, T right) => left.Add(right);
        public static T Multiply(T vector, F scalar) => vector.Multiply(scalar);
        public static T Subtract(T left, T right) => left.Subtract(right);
        public static T Divide(T vector, F scalar) => vector.Divide(scalar);

        public static T operator +(LinearSpace<T, F> left, T right) => left.Add(right);
        public static T operator -(LinearSpace<T, F> left, T right) => left.Subtract(right);

        public static T operator *(LinearSpace<T, F> vector, F scalar) => vector.Multiply(scalar);
        public static T operator *(F scalar, LinearSpace<T, F> vector) => vector.Multiply(scalar);
        public static T operator /(LinearSpace<T, F> vector, F scalar) => vector.Divide(scalar);

        public static T operator -(LinearSpace<T, F> vector) => vector.AdditiveInverse;


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

        public static bool operator ==(LinearSpace<T, F> left, T right) => left.Equals(right);
        public static bool operator !=(LinearSpace<T, F> left, T right) => !left.Equals(right);
    }

#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
}