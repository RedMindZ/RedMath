using RedMath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public abstract class DynamicDimensionCoordinateSpace<T, F> : CoordinateSpace<T, F> where F : Field<F>, new() where T : DynamicDimensionCoordinateSpace<T, F>, new()
    {
        public override int Dimension => Elements.Length;

        protected DynamicDimensionCoordinateSpace()
        {
            Elements = new F[1];
            Elements.AssignAll((ind) => Field<F>.Zero);
        }

        protected DynamicDimensionCoordinateSpace(int dimension)
        {
            Elements = new F[dimension];
            Elements.AssignAll((ind) => Field<F>.Zero);
        }

        protected DynamicDimensionCoordinateSpace(params F[] elements)
        {
            Elements = new F[elements.Length];
            Elements.AssignAll((ind) => elements[ind[0]].Clone());
        }

        public override T Add(T other)
        {
            if (Dimension != other.Dimension)
            {
                throw new ArgumentException("Vectors with different dimensions can't be added.");
            }

            return base.Add(other);
        }

        public override bool Equals(T other)
        {
            if (Dimension != other.Dimension)
            {
                return false;
            }

            return base.Equals(other);
        }
    }
}
