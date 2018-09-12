using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures.LinearSpaces
{
    public class RealVector3 : StaticDimensionCoordinateSpace<RealVector3, Real>
    {
        public override int Dimension => 3;

        public RealVector3() : base() { }
        public RealVector3(Real x, Real y, Real z) : base(x, y, z) { }
    }
}
