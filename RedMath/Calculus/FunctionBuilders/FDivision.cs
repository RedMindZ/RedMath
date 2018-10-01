using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public class FDivision<D, R> : IOptimizableFunction<D, R, R>
        where R : Field<R>, new()
    {
        public IOptimizableFunction<D, R, R> LeftFunction { get; }
        public IOptimizableFunction<D, R, R> RightFunction { get; }
        public int ParametersCount { get; }

        public FDivision(IOptimizableFunction<D, R, R> leftFunction, IOptimizableFunction<D, R, R> rightFunction)
        {
            LeftFunction = leftFunction;
            RightFunction = rightFunction;
            ParametersCount = LeftFunction.ParametersCount + RightFunction.ParametersCount;
        }

        public R Compute(D input)
        {
            return LeftFunction.Compute(input).Divide(RightFunction.Compute(input));
        }

        public Matrix<R> ComputeParametersJacobian(D input)
        {
            var leftJacobian = LeftFunction.ComputeParametersJacobian(input);
            var rightJacobian = RightFunction.ComputeParametersJacobian(input);

            var scalar = RightFunction.Compute(input).MultiplicativeInverse;
            leftJacobian *= scalar;
            rightJacobian *= -LeftFunction.Compute(input) * scalar * scalar;

            return Matrix<R>.ConcatRows(leftJacobian, rightJacobian);
        }

        public Matrix<R> ComputeVariablesJacobian(D input)
        {
            var leftJacobian = LeftFunction.ComputeParametersJacobian(input);
            var rightJacobian = RightFunction.ComputeParametersJacobian(input);

            var scalar = RightFunction.Compute(input).MultiplicativeInverse;
            leftJacobian *= scalar;
            rightJacobian *= -LeftFunction.Compute(input) * scalar * scalar;

            return leftJacobian - rightJacobian;
        }

        public void GetAllParameters(R[] buffer, int startIndex)
        {
            LeftFunction.GetAllParameters(buffer, startIndex);
            RightFunction.GetAllParameters(buffer, startIndex + LeftFunction.ParametersCount);
        }

        public void SetAllParameters(R[] paramValues, int startIndex)
        {
            LeftFunction.SetAllParameters(paramValues, startIndex);
            RightFunction.SetAllParameters(paramValues, startIndex + LeftFunction.ParametersCount);
        }

        public void GetAllParametersOptimizationScales(R[] buffer, int startIndex)
        {
            LeftFunction.GetAllParametersOptimizationScales(buffer, startIndex);
            RightFunction.GetAllParametersOptimizationScales(buffer, startIndex + LeftFunction.ParametersCount);
        }
    }
}
