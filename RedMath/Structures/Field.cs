using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    /*[Obsolete("This is an old implementation that should not be used. Use the 'Field<T>' class instead.", false)]
    public abstract class Field<T> : IEquatable<T> where T : new()
    {
        public abstract T Zero { get; }
        public abstract T One { get; }

        public abstract T AdditiveInverse { get; }
        public abstract T MultiplicativeInverse { get; }

        public abstract T Add(T element);
        public abstract T Multiply(T element);

        public Field()
        {
            if (!GetType().IsSubclassOf(typeof(T)))
            {
                throw new ArgumentException("The type passed to 'Field<T>' must be the same as the type of the derived class. Any class that derives from 'Field<T>' should take the form 'MyClass : Field<MyClass>'", "T");
            }
        }
        
        public T Subtract(Field<T> element)
        {
            return Add(element.AdditiveInverse);
        }

        public T Divide(Field<T> element)
        {
            return Multiply(element.MultiplicativeInverse);
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
    }*/

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


        public static T Add(T lhs, T rhs)
        {
            return lhs.Add(rhs);
        }

        public static T Multiply(T lhs, T rhs)
        {
            return lhs.Multiply(rhs);
        }

        public static T Subtract(T lhs, T rhs)
        {
            return lhs.Subtract(rhs);
        }

        public static T Divide(T lhs, T rhs)
        {
            return lhs.Divide(rhs);
        }



        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj))
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
