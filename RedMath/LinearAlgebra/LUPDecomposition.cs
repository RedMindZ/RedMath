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
            UpperMatrix = new Matrix<T>(Math.Min(mat.Rows, mat.Columns), mat.Columns);
            LowerMatrix = new Matrix<T>(mat.Rows, Math.Min(mat.Rows, mat.Columns));
            Permutation = new RowPermutation<T>();

            List<MatrixOperation<T>> reductionOps = new List<MatrixOperation<T>>(mat.EchelonFormReductionOperations);
            reductionOps.Reverse();

            UpperMatrix = mat.EchelonForm;

            Field<T> fieldOne = new T().One;
            for (int i = 0; i < Math.Min(LowerMatrix.Rows, LowerMatrix.Columns); i++)
            {
                LowerMatrix[i, i] = fieldOne.Clone();
            }

            int[] permArr = new int[mat.Rows];
            for (int i = 0; i < mat.Rows; i++)
            {
                permArr[i] = i;
            }

            Permutation.IndexList = new List<int>(permArr);

            foreach (MatrixOperation<T> op in reductionOps)
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
