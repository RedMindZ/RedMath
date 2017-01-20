using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public abstract class Field<T> : IEquatable<T> where T : new()
    {
        public abstract T Zero { get; }
        public abstract T One { get; }

        public abstract T AdditiveInverse { get; }
        public abstract T MultiplicativeInverse { get; }

        public abstract T Add(T element);
        public abstract T Multiply(T element);

        public abstract bool Equals(T other);

        public T Subtract(Field<T> element)
        {
            return this.Add(element.AdditiveInverse);
        }

        public T Divide(Field<T> element)
        {
            return this.Multiply(element.MultiplicativeInverse);
        }
    }
}
