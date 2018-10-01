using RedMath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public abstract class CoordinateSpace<T, F> : LinearSpace<T, F> where T : CoordinateSpace<T, F>, new() where F : Field<F>, new()
    {
        public F[] Elements { get; protected set; }
        public abstract int Dimension { get; }

        protected override T Zero
        {
            get
            {
                T zero = CreateEmptyVector(Dimension);
                zero.Elements.AssignAll((ind) => Field<F>.Zero);

                return zero;
            }
        }

        protected override F UnitScalar => Field<F>.One;

        public override T AdditiveInverse
        {
            get
            {
                T inverse = CreateEmptyVector(Dimension);
                inverse.Elements.AssignAll((ind) => -Elements[ind[0]]);

                return inverse;
            }
        }

        public F this[int index] { get => Elements[index]; set => Elements[index] = value; }

        public override T Add(T other)
        {
            T sum = CreateEmptyVector(Dimension);
            sum.Elements.AssignAll((ind) => Elements[ind[0]] + other.Elements[ind[0]]);

            return sum;
        }

        public override T Multiply(F scalar)
        {
            T prod = CreateEmptyVector(Dimension);
            prod.Elements.AssignAll((ind) => Elements[ind[0]] * scalar);

            return prod;
        }

        public override T Clone()
        {
            T clone = CreateEmptyVector(Dimension);
            clone.Elements.AssignAll((ind) => Elements[ind[0]].Clone());

            return clone;
        }

        public override bool Equals(T other)
        {
            for (int i = 0; i < Dimension; i++)
            {
                if (Elements[i] != other.Elements[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return "(" + string.Join<F>(", ", Elements) + ")";
        }

        private static T CreateEmptyVector(int dimension)
        {
            T vec = new T()
            {
                Elements = new F[dimension]
            };

            return vec;
        }
    }
}
