using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMath.Calculus
{
    public interface IDifferentiableFunction<Domain, Range, DomainField> : IFunction<Domain, Range>
        where DomainField : Field<DomainField>, new()
    {
        Matrix<DomainField> ComputeJacobianWrtInput(Domain input);
    }
}
