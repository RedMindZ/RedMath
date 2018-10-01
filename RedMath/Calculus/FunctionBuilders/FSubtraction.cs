using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    /// <summary>
    /// Subtracts two <see cref="IOptimizableFunction{D, R, PF}"/> functions.
    /// </summary>
    /// <typeparam name="D">The domain of the functions.</typeparam>
    /// <typeparam name="R">The range of the functions.</typeparam>
    /// <typeparam name="PF">The type of the field of the parameters of the functions.</typeparam>
    public class FSubtraction<D, R, PF> : IOptimizableFunction<D, R, PF>
        where R : ISubtractable<R>
        where PF : Field<PF>, new()
    {
        public IOptimizableFunction<D, R, PF> LeftFunction { get; }
        public IOptimizableFunction<D, R, PF> RightFunction { get; }
        public int ParametersCount { get; }

        public FSubtraction(IOptimizableFunction<D, R, PF> leftFunction, IOptimizableFunction<D, R, PF> rightFunction)
        {
            LeftFunction = leftFunction;
            RightFunction = rightFunction;
            ParametersCount = LeftFunction.ParametersCount + RightFunction.ParametersCount;
        }

        public R Compute(D input)
        {
            return LeftFunction.Compute(input).Subtract(RightFunction.Compute(input));
        }

        public Matrix<PF> ComputeParametersJacobian(D input)
        {
            return Matrix<PF>.ConcatRows(LeftFunction.ComputeParametersJacobian(input), -RightFunction.ComputeParametersJacobian(input));
        }

        public Matrix<PF> ComputeVariablesJacobian(D input)
        {
            return LeftFunction.ComputeVariablesJacobian(input) - RightFunction.ComputeVariablesJacobian(input);
        }

        public void GetAllParameters(PF[] buffer, int startIndex)
        {
            LeftFunction.GetAllParameters(buffer, startIndex);
            RightFunction.GetAllParameters(buffer, startIndex + LeftFunction.ParametersCount);
        }

        public void SetAllParameters(PF[] paramValues, int startIndex)
        {
            LeftFunction.SetAllParameters(paramValues, startIndex);
            RightFunction.SetAllParameters(paramValues, startIndex + LeftFunction.ParametersCount);
        }

        public void GetAllParametersOptimizationScales(PF[] buffer, int startIndex)
        {
            LeftFunction.GetAllParametersOptimizationScales(buffer, startIndex);
            RightFunction.GetAllParametersOptimizationScales(buffer, startIndex + LeftFunction.ParametersCount);
        }
    }
}
