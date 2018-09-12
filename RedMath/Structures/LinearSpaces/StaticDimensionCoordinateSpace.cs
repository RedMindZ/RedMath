using RedMath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public abstract class StaticDimensionCoordinateSpace<T, F> : CoordinateSpace<T, F> where T : StaticDimensionCoordinateSpace<T, F>, new() where F : Field<F>, new()
    {
        protected StaticDimensionCoordinateSpace()
        {
            Elements = new F[Dimension];
            Elements.AssignAll((ind) => Field<F>.Zero);
        }

        protected StaticDimensionCoordinateSpace(params F[] elements)
        {
            if (elements.Length != Dimension)
            {
                throw new ArgumentException("The number of elements must be equal to '" + nameof(Dimension) + "'.");
            }

            Elements = new F[Dimension];
            Elements.AssignAll((ind) => elements[ind[0]].Clone());
        }
    }
}
