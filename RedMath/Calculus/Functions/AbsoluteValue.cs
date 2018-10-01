using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;

namespace RedMath.Calculus
{
    public class AbsoluteValue : IOptimizableFunction<Real, Real, Real>
    {
        public int ParametersCount => 0;

        public Real Compute(Real input)
        {
            return Math.Abs(input);
        }

        public Matrix<Real> ComputeParametersJacobian(Real input) => new Matrix<Real>();
        public Matrix<Real> ComputeVariablesJacobian(Real input) => new Matrix<Real>(new Real[,] { { Math.Sign(input) } });

        public void GetAllParameters(Real[] buffer, int startIndex) { }
        public void SetAllParameters(Real[] paramValues, int startIndex) { }
        public void GetAllParametersOptimizationScales(Real[] buffer, int startIndex) { }
    }
}
