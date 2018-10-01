using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMath.Calculus
{
    /// <summary>
    /// Multiplies a variable number of <see cref="IOptimizableFunction{D, R, PF}"/> functions.
    /// </summary>
    /// <typeparam name="D">The domain of the functions.</typeparam>
    /// <typeparam name="R">The range of the functions.</typeparam>
    /// <typeparam name="PF">The type of the field of the parameters of the functions.</typeparam>
    public class FMultiplication<D, R> : IOptimizableFunction<D, R, R>
        where R : Field<R>, new()
    {
        private int[] _parametersOffsets;

        public ReadOnlyCollection<IOptimizableFunction<D, R, R>> Functions { get; }
        public int ParametersCount { get; }

        public FMultiplication(params IOptimizableFunction<D, R, R>[] functions)
        {
            if (functions.Length == 0)
            {
                throw new ArgumentException("Zero functions can't be summed.");
            }

            Functions = new ReadOnlyCollection<IOptimizableFunction<D, R, R>>(functions.ToArray());
            ParametersCount = Functions.Sum((f) => f.ParametersCount);

            _parametersOffsets = new int[Functions.Count];
            for (int i = 1; i < Functions.Count; i++)
            {
                _parametersOffsets[i] = _parametersOffsets[i - 1] + Functions[i].ParametersCount;
            }
        }

        public R Compute(D input)
        {
            R sum = Functions[0].Compute(input);

            for (int i = 1; i < Functions.Count; i++)
            {
                sum = sum.Multiply(Functions[i].Compute(input));
            }

            return sum;
        }

        public Matrix<R> ComputeParametersJacobian(D input)
        {
            Matrix<R>[] jacobs = new Matrix<R>[Functions.Count];

            for (int i = 0; i < jacobs.Length; i++)
            {
                jacobs[i] = Functions[i].ComputeParametersJacobian(input);
            }

            R scalar = Field<R>.One;

            for (int i = 0; i < Functions.Count; i++)
            {
                scalar *= Functions[i].Compute(input);
            }

            for (int i = 0; i < jacobs.Length; i++)
            {
                jacobs[i] *= scalar / Functions[i].Compute(input);
            }

            return Matrix<R>.ConcatRows(jacobs);
        }

        public Matrix<R> ComputeVariablesJacobian(D input)
        {
            Matrix<R>[] jacobs = new Matrix<R>[Functions.Count];

            for (int i = 0; i < jacobs.Length; i++)
            {
                jacobs[i] = Functions[i].ComputeParametersJacobian(input);
            }

            R scalar = Field<R>.One;

            for (int i = 0; i < Functions.Count; i++)
            {
                scalar *= Functions[i].Compute(input);
            }

            Matrix<R> jacobsSum = jacobs[0] * (scalar / Functions[0].Compute(input));

            for (int i = 1; i < jacobs.Length; i++)
            {
                jacobsSum += jacobs[i] * (scalar / Functions[i].Compute(input));
            }

            return jacobs.Aggregate((mat1, mat2) => mat1 + mat2);
        }

        public void GetAllParameters(R[] buffer, int startIndex)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                Functions[i].GetAllParameters(buffer, startIndex + _parametersOffsets[i]);
            }
        }

        public void SetAllParameters(R[] paramValues, int startIndex)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                Functions[i].SetAllParameters(paramValues, startIndex + _parametersOffsets[i]);
            }
        }

        public void GetAllParametersOptimizationScales(R[] buffer, int startIndex)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                Functions[i].GetAllParametersOptimizationScales(buffer, startIndex + _parametersOffsets[i]);
            }
        }
    }
}
