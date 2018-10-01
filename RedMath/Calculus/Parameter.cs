using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public class Parameter<F> where F : Field<F>, new()
    {
        public F Value { get; set; }
        public F OptimizationScale { get; set; }

        public static implicit operator F(Parameter<F> p) => p.Value;
    }
}
