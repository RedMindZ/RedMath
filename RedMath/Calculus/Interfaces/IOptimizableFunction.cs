using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedMath.Structures;
using RedMath.LinearAlgebra;

namespace RedMath.Calculus
{
    /// <summary>
    /// Provides a mechanism to optimize the parameters of a function.
    /// </summary>
    /// <typeparam name="D">The domain of the function.</typeparam>
    /// <typeparam name="R">The range of the function.</typeparam>
    /// <typeparam name="PF">The type of the field of the parameters of the function.</typeparam>
    public interface IOptimizableFunction<D, R, PF> : IFunction<D, R>
        where PF : Field<PF>, new()
    {
        /// <summary>
        /// The number of parameters the function contains.
        /// </summary>
        int ParametersCount { get; }

        /// <summary>
        /// Computes the jacobian of the function with respect to the internal function parameters.
        /// </summary>
        /// <param name="input">The input at which to compute the jacobian.</param>
        /// <returns>The jacobian.</returns>
        Matrix<PF> ComputeParametersJacobian(D input);

        /// <summary>
        /// Computes the jacobian of the function with respect to the input variables.
        /// </summary>
        /// <param name="input">The input at which to compute the jacobian.</param>
        /// <returns>The jacobian.</returns>
        Matrix<PF> ComputeVariablesJacobian(D input);

        /// <summary>
        /// Writes the parameters of the function to <paramref name="buffer"/> starting from <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="buffer">The buffer to write the parameters to.</param>
        /// <param name="startIndex">The index of <paramref name="buffer"/> at which to start writing.</param>
        void GetAllParameters(PF[] buffer, int startIndex);

        /// <summary>
        /// Sets the parameters of the function to the values of <paramref name="buffer"/> starting from <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="buffer">The buffer from which to read the new parameter values.</param>
        /// <param name="startIndex">The index of <paramref name="buffer"/> from which to start reading.</param>
        void SetAllParameters(PF[] paramValues, int startIndex);

        /// <summary>
        /// Writes the optimization scale of each parameter to <paramref name="buffer"/> starting from <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="buffer">The buffer to write the optimization scales to.</param>
        /// <param name="startIndex">The index of <paramref name="buffer"/> at which to start writing.</param>
        void GetAllParametersOptimizationScales(PF[] buffer, int startIndex);
    }
}
