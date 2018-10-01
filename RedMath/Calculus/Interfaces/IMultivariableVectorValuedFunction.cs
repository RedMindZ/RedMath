using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public interface IMultivariableVectorValuedFunction<DomainSpace, DomainField, RangeSpace, RangeField> : IMultivariableFunction<DomainSpace, DomainField, RangeSpace>, IVectorValuedFunction<DomainSpace, RangeSpace, RangeField>
        where DomainField : Field<DomainField>, new()
        where DomainSpace : LinearSpace<DomainSpace, DomainField>, new()
        where RangeField : Field<RangeField>, new()
        where RangeSpace : LinearSpace<RangeSpace, RangeField>, new()
    { }
}
