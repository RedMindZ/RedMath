using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMath.Calculus
{
    public class RmmMultiplication<F> : IOptimizableFunction<Matrix<F>, Matrix<F>, F>
        where F : Field<F>, new()
    {
        public Matrix<F> ParametersMatrix { get; }
        public int ParametersCount { get; }

        public RmmMultiplication(Matrix<F> parametersMatrix)
        {
            ParametersMatrix = new Matrix<F>(parametersMatrix);
            ParametersCount = ParametersMatrix.Rows * ParametersMatrix.Columns;
        }

        public Matrix<F> Compute(Matrix<F> input)
        {
            return input * ParametersMatrix;
        }

        public Matrix<F> ComputeParametersJacobian(Matrix<F> input)
        {
            var jacob = new Matrix<F>(input.Rows * ParametersMatrix.Columns, ParametersMatrix.Rows * ParametersMatrix.Columns);

            for (int i = 0; i < ParametersMatrix.Rows; i++)
            {
                for (int j = 0; j < ParametersMatrix.Columns; j++)
                {
                    for (int k = 0; k < input.Rows; k++)
                    {
                        var col = i * ParametersMatrix.Columns + j;
                        var row = j + k * ParametersMatrix.Columns;
                        jacob[row, col] = input[k, i];
                    }
                }
            }

            return jacob;
        }

        public Matrix<F> ComputeVariablesJacobian(Matrix<F> input)
        {
            var jacob = new Matrix<F>(input.Rows * ParametersMatrix.Columns, input.Rows * input.Columns);

            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Columns; j++)
                {
                    for (int k = 0; k < ParametersMatrix.Columns; k++)
                    {
                        var col = i * input.Columns + j;
                        var row = k + i * ParametersMatrix.Columns;
                        jacob[row, col] = ParametersMatrix[j, k];
                    }
                }
            }

            return jacob;
        }

        public void GetAllParameters(F[] buffer, int startIndex)
        {
            for (int i = 0; i < ParametersMatrix.Rows; i++)
            {
                for (int j = 0; j < ParametersMatrix.Columns; j++)
                {
                    buffer[startIndex + j + i * ParametersMatrix.Columns] = ParametersMatrix[i, j];
                }
            }
        }

        public void SetAllParameters(F[] paramValues, int startIndex)
        {
            for (int i = 0; i < ParametersMatrix.Rows; i++)
            {
                for (int j = 0; j < ParametersMatrix.Columns; j++)
                {
                    ParametersMatrix[i, j] = paramValues[startIndex + j + i * ParametersMatrix.Columns];
                }
            }
        }

        public void GetAllParametersOptimizationScales(F[] buffer, int startIndex)
        {
            for (int i = 0; i < ParametersMatrix.Rows; i++)
            {
                for (int j = 0; j < ParametersMatrix.Columns; j++)
                {
                    buffer[startIndex + j + i * ParametersMatrix.Columns] = Field<F>.One;
                }
            }
        }
    }
}
