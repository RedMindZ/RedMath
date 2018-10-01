using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Calculus
{
    public static class SGD
    {
        public static void OptimizeParameters<ParametersField>(ParametersField[] parameters, Matrix<ParametersField> jacobian, ParametersField[] optimizationScales, ParametersField scale)
            where ParametersField : Field<ParametersField>, new()
        {
            for (int j = 0; j < parameters.Length; j++)
            {
                parameters[j] -= jacobian.GetColumnVector(j).Sum() * optimizationScales[j] * scale;
            }
        }

        public static void MinimizeFunctionValue<Domain, Range, ParametersField>(IOptimizableFunction<Domain, Range, ParametersField> func, Domain input, ParametersField scale)
            where ParametersField : Field<ParametersField>, new()
        {
            var parameters = new ParametersField[func.ParametersCount];
            func.GetAllParameters(parameters, 0);

            var optimizationScales = new ParametersField[func.ParametersCount];
            func.GetAllParametersOptimizationScales(optimizationScales, 0);

            var jacob = func.ComputeParametersJacobian(input);

            OptimizeParameters(parameters, jacob, optimizationScales, scale);

            func.SetAllParameters(parameters, 0);
        }

        public static void MinimizeFunctionValue<Domain, Range, ParametersField>(IOptimizableFunction<Domain, Range, ParametersField> func, Domain input, ParametersField scale, int iterationsCount)
            where ParametersField : Field<ParametersField>, new()
        {
            var parameters = new ParametersField[func.ParametersCount];
            func.GetAllParameters(parameters, 0);

            var optimizationScales = new ParametersField[func.ParametersCount];
            func.GetAllParametersOptimizationScales(optimizationScales, 0);

            for (int i = 0; i < iterationsCount; i++)
            {
                var jacob = func.ComputeParametersJacobian(input);

                OptimizeParameters(parameters, jacob, optimizationScales, scale);

                func.SetAllParameters(parameters, 0);
            }
        }
    }
}
