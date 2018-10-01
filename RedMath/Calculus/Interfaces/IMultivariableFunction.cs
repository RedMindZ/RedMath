using RedMath.Structures;
using RedMath.LinearAlgebra;
using RedMath.Utils;

namespace RedMath.Calculus
{
    public interface IMultivariableFunction<DomainSpace, DomainField, Range> : IFunction<DomainSpace, Range>
        where DomainField : Field<DomainField>, new()
        where DomainSpace : LinearSpace<DomainSpace, DomainField>, new()
    { }
}
