using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public class Monomial : IOptimizableFunction<Real, Real, Real>
    {
        public Real Coefficient { get; set; }
        public Real CoefficientOptimizationScale { get; set; } = 1;

        public Real Power { get; set; }
        public Real PowerOptimizationScale { get; set; } = 1;

        public int ParametersCount => 2;

        public Monomial(Real coefficient, Real power)
        {
            Coefficient = coefficient;
            Power = power;
        }

        public Real Compute(Real input)
        {
            return Coefficient * Math.Pow(input, Power);
        }

        public Matrix<Real> ComputeParametersJacobian(Real input)
        {
            return new Matrix<Real>
            (
                new Real[,]
                {
                    {
                        Math.Pow(input, Power),
                        Coefficient * Math.Log(input) * Math.Pow(input, Power)
                    }
                }
            );
        }

        public Matrix<Real> ComputeVariablesJacobian(Real input)
        {
            return new Matrix<Real>(new Real[,] { { Power * Coefficient * Math.Pow(input, Power - 1) } });
        }

        public void GetAllParameters(Real[] buffer, int startIndex)
        {
            buffer[startIndex] = Coefficient;
            buffer[startIndex + 1] = Power;
        }

        public void SetAllParameters(Real[] paramValues, int startIndex)
        {
            Coefficient = paramValues[startIndex];
            Power = paramValues[startIndex + 1];
        }

        public void GetAllParametersOptimizationScales(Real[] buffer, int startIndex)
        {
            buffer[startIndex] = CoefficientOptimizationScale;
            buffer[startIndex + 1] = PowerOptimizationScale;
        }
    }
}
