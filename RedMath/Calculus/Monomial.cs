using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public class Monomial : Function<Real, Real>, IDifferentiableFunction<Monomial, Real, Real>
    {
        public Real Power { get; set; }
        public Real Coefficient { get; set; }

        public Monomial Derivative => new Monomial(Power - 1, Coefficient * Power);

        public Monomial(Real power, Real coefficient)
        {
            Power = power;
            Coefficient = coefficient;
        }

        public override Real Compute(Real input)
        {
            return Math.Pow(Coefficient * input, Power);
        }
    }
}
