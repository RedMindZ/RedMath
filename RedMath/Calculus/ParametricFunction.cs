using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public abstract class ParametricFunction<DomainType, RangeType, ParametersType> : Function<DomainType, RangeType>
    {
        public abstract ParametersType[] Parameters { get; }
        public abstract int ParametersCount { get; }
    }
}
