using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMath.Calculus
{
    public class LmmMultiplication<F> : IOptimizableFunction<Matrix<F>, Matrix<F>, F>
        where F : Field<F>, new()
    {
        public Matrix<F> ParametersMatrix { get; }
        public int ParametersCount { get; }

        public LmmMultiplication(Matrix<F> parametersMatrix)
        {
            ParametersMatrix = new Matrix<F>(parametersMatrix);
            ParametersCount = ParametersMatrix.Rows * ParametersMatrix.Columns;
        }

        public Matrix<F> Compute(Matrix<F> input)
        {
            return ParametersMatrix * input;
        }

        public Matrix<F> ComputeParametersJacobian(Matrix<F> input)
        {
            var jacob = new Matrix<F>(ParametersMatrix.Rows * input.Columns, ParametersMatrix.Rows * ParametersMatrix.Columns);

            for (int blockIndex = 0; blockIndex < ParametersMatrix.Rows; blockIndex++)
            {
                for (int i = 0; i < input.Columns; i++)
                {
                    for (int j = 0; j < input.Rows; j++)
                    {
                        jacob[i + blockIndex * input.Columns, j + blockIndex * input.Rows] = input[j, i];
                    }
                }
            }

            return jacob;
        }

        public Matrix<F> ComputeVariablesJacobian(Matrix<F> input)
        {
            var jacob = new Matrix<F>(ParametersMatrix.Rows * input.Columns, input.Rows * input.Columns);

            for (int i = 0; i < input.Columns; i++)
            {
                for (int j = 0; j < input.Rows; j++)
                {
                    var col = i + j * input.Columns;

                    for (int k = 0; k < ParametersMatrix.Rows; k++)
                    {
                        var row = i + k * input.Columns;
                        jacob[row, col] = ParametersMatrix[k, j];
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
