using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RedMath.Calculus
{
    public interface IDifferentiableFunction<DerivativeType, DomainType, RangeType> where DerivativeType : Function<DomainType, RangeType>
    {
        DerivativeType Derivative { get; }
    }
}
