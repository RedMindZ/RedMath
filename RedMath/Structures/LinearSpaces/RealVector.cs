using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures.LinearSpaces
{
    public class RealVector : DynamicDimensionCoordinateSpace<RealVector, Real>
    {
        public RealVector() : base() { }
        public RealVector(int dimension) : base(dimension) { }
        public RealVector(params Real[] elements) : base(elements) { }
    }
}
