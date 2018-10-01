using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public interface IVectorValuedFunction<Domain, RangeSpace, RangeField> : IFunction<Domain, RangeSpace>
        where RangeField : Field<RangeField>, new()
        where RangeSpace : LinearSpace<RangeSpace, RangeField>, new()
    { }
}
