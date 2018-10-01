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
    /// Composes two functions.
    /// </summary>
    /// <typeparam name="IFD">Inner Function Domain - The type of the domain of the inner function.</typeparam>
    /// <typeparam name="SRD">Shared Range/Domain - The type of the range/domain that is shared between the inner and outer function.</typeparam>
    /// <typeparam name="OFR">Outer Function Range - The type of the range of the outer function.</typeparam>
    /// <typeparam name="PF">Parameters Field - The type of the field of the parameters of the function.</typeparam>
    public class FComposition<IFD, SRD, OFR, PF> : IOptimizableFunction<IFD, OFR, PF>
        where PF : Field<PF>, new()
    {
        public IOptimizableFunction<SRD, OFR, PF> OuterFunction { get; }
        public IOptimizableFunction<IFD, SRD, PF> InnerFunction { get; }
        public int ParametersCount { get; }

        public FComposition(IOptimizableFunction<SRD, OFR, PF> outerFunction, IOptimizableFunction<IFD, SRD, PF> innerFunction)
        {
            OuterFunction = outerFunction;
            InnerFunction = innerFunction;
            ParametersCount = OuterFunction.ParametersCount + InnerFunction.ParametersCount;
        }

        public OFR Compute(IFD input)
        {
            return OuterFunction.Compute(InnerFunction.Compute(input));
        }

        public Matrix<PF> ComputeParametersJacobian(IFD input)
        {

            if (OuterFunction.ParametersCount > 0 && InnerFunction.ParametersCount > 0)
            {
                Matrix<PF> outerParamsJacobian = OuterFunction.ComputeParametersJacobian(InnerFunction.Compute(input));
                Matrix<PF> outerVarsJacobian = OuterFunction.ComputeVariablesJacobian(InnerFunction.Compute(input));
                Matrix<PF> innerParamsJacobian = InnerFunction.ComputeParametersJacobian(input);

                return Matrix<PF>.ConcatRows(outerParamsJacobian, outerVarsJacobian * innerParamsJacobian);
            }
            else if (OuterFunction.ParametersCount == 0)
            {
                Matrix<PF> outerVarsJacobian = OuterFunction.ComputeVariablesJacobian(InnerFunction.Compute(input));
                Matrix<PF> innerParamsJacobian = InnerFunction.ComputeParametersJacobian(input);

                return outerVarsJacobian * innerParamsJacobian;
            }
            else
            {
                return OuterFunction.ComputeParametersJacobian(InnerFunction.Compute(input));
            }
        }

        public Matrix<PF> ComputeVariablesJacobian(IFD input)
        {
            return OuterFunction.ComputeVariablesJacobian(InnerFunction.Compute(input)) * InnerFunction.ComputeVariablesJacobian(input);
        }

        public void GetAllParameters(PF[] buffer, int startIndex)
        {
            OuterFunction.GetAllParameters(buffer, startIndex);
            InnerFunction.GetAllParameters(buffer, startIndex + OuterFunction.ParametersCount);
        }

        public void SetAllParameters(PF[] paramValues, int startIndex)
        {
            OuterFunction.SetAllParameters(paramValues, startIndex);
            InnerFunction.SetAllParameters(paramValues, startIndex + OuterFunction.ParametersCount);
        }

        public void GetAllParametersOptimizationScales(PF[] buffer, int startIndex)
        {
            OuterFunction.GetAllParametersOptimizationScales(buffer, startIndex);
            InnerFunction.GetAllParametersOptimizationScales(buffer, startIndex + OuterFunction.ParametersCount);
        }
    }
}
