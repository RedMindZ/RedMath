using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public abstract class Function<DomainType, RangeType>
    {
        public abstract RangeType Compute(DomainType input);
    }
}
