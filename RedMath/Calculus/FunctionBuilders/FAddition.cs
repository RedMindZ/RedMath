using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    /// <summary>
    /// Sums a variable number of functions.
    /// </summary>
    /// <typeparam name="D">The domain of the functions.</typeparam>
    /// <typeparam name="R">The range of the functions.</typeparam>
    public class FAddition<D, R> : IFunction<D, R> where R : IAddable<R>
    {
        public ReadOnlyCollection<IFunction<D, R>> Functions { get; }

        public FAddition(params IFunction<D, R>[] functions)
        {
            if (functions.Length == 0)
            {
                throw new ArgumentException("Zero functions can't be summed.");
            }

            Functions = new ReadOnlyCollection<IFunction<D, R>>(functions.ToArray());
        }

        public R Compute(D input)
        {
            R sum = Functions[0].Compute(input);

            for (int i = 1; i < Functions.Count; i++)
            {
                sum = sum.Add(Functions[i].Compute(input));
            }

            return sum;
        }
    }

    /// <summary>
    /// Sums a variable number of <see cref="IOptimizableFunction{D, R, PF}"/> functions.
    /// </summary>
    /// <typeparam name="D">The domain of the functions.</typeparam>
    /// <typeparam name="R">The range of the functions.</typeparam>
    /// <typeparam name="PF">The type of the field of the parameters of the functions.</typeparam>
    public class FAddition<D, R, PF> : IOptimizableFunction<D, R, PF>
        where R : IAddable<R>
        where PF : Field<PF>, new()
    {
        private int[] _parametersOffsets;

        public ReadOnlyCollection<IOptimizableFunction<D, R, PF>> Functions { get; }
        public int ParametersCount { get; }

        public FAddition(params IOptimizableFunction<D, R, PF>[] functions)
        {
            if (functions.Length == 0)
            {
                throw new ArgumentException("Zero functions can't be summed.");
            }

            Functions = new ReadOnlyCollection<IOptimizableFunction<D, R, PF>>(functions.ToArray());
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
                sum = sum.Add(Functions[i].Compute(input));
            }

            return sum;
        }

        public Matrix<PF> ComputeParametersJacobian(D input)
        {
            Matrix<PF>[] jacobs = new Matrix<PF>[Functions.Count];

            for (int i = 0; i < jacobs.Length; i++)
            {
                jacobs[i] = Functions[i].ComputeParametersJacobian(input);
            }

            return Matrix<PF>.ConcatRows(jacobs);
        }

        public Matrix<PF> ComputeVariablesJacobian(D input)
        {
            Matrix<PF> jacob = Functions[0].ComputeVariablesJacobian(input);

            for (int i = 1; i < Functions.Count; i++)
            {
                jacob += Functions[i].ComputeParametersJacobian(input);
            }

            return jacob;
        }

        public void GetAllParameters(PF[] buffer, int startIndex)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                Functions[i].GetAllParameters(buffer, startIndex + _parametersOffsets[i]);
            }
        }

        public void SetAllParameters(PF[] paramValues, int startIndex)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                Functions[i].SetAllParameters(paramValues, startIndex + _parametersOffsets[i]);
            }
        }

        public void GetAllParametersOptimizationScales(PF[] buffer, int startIndex)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                Functions[i].GetAllParametersOptimizationScales(buffer, startIndex + _parametersOffsets[i]);
            }
        }
    }
}