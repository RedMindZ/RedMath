using System;
using System.Collections.Generic;

using RedMath.Structures;
using RedMath.LinearAlgebra.MatrixOperations;

namespace RedMath.LinearAlgebra
{
    public class LUPDecomposition<T> where T : Field<T>, new()
    {
        public Matrix<T> UpperMatrix { get; private set; }
        public Matrix<T> LowerMatrix { get; private set; }
        public RowPermutation<T> Permutation { get; private set; }

        public LUPDecomposition(Matrix<T> mat)
        {
            UpperMatrix = mat.EchelonForm;

            LowerMatrix = new Matrix<T>(mat.Rows, Math.Min(mat.Rows, mat.Columns));

            List<IMatrixOperation<T>> reductionOps = new List<IMatrixOperation<T>>(mat.EchelonFormReductionOperations);
            reductionOps.Reverse();

            for (int i = 0; i < Math.Min(LowerMatrix.Rows, LowerMatrix.Columns); i++)
            {
                LowerMatrix[i, i] = Field<T>.One;
            }

            int[] permArr = new int[mat.Rows];
            for (int i = 0; i < mat.Rows; i++)
            {
                permArr[i] = i;
            }

            Permutation = new RowPermutation<T>(permArr);

            foreach (IMatrixOperation<T> op in reductionOps)
            {
                if (op is SwapRows<T> swapOp)
                {
                    int temp = Permutation.IndexList[swapOp.FirstRowIndex];
                    Permutation.IndexList[swapOp.FirstRowIndex] = Permutation.IndexList[swapOp.SecondRowIndex];
                    Permutation.IndexList[swapOp.SecondRowIndex] = temp;
                }
                else
                {
                    op.InverseApplyTo(LowerMatrix); 
                }
            }
        }

        public LUPDecomposition(LUPDecomposition<T> other)
        {
            UpperMatrix = new Matrix<T>(other.UpperMatrix);
            LowerMatrix = new Matrix<T>(other.LowerMatrix);
            Permutation = new RowPermutation<T>(other.Permutation.IndexList.ToArray());
        }
    }
}
