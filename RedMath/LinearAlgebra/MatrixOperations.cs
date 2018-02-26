using System;
using System.Collections.Generic;

using RedMath.Structures;

namespace RedMath.LinearAlgebra.MatrixOperations
{
    public interface MatrixOperation<T> where T : Field<T>, new()
    {
        void ApplyTo(Matrix<T> target);
        void InverseApplyTo(Matrix<T> target);
    }

    public class SwapRows<T> : MatrixOperation<T> where T : Field<T>, new()
    {
        public int FirstRowIndex { get; set; }
        public int SecondRowIndex { get; set; }

        public SwapRows(int firstRowIndex, int secondRowIndex)
        {
            FirstRowIndex = firstRowIndex;
            SecondRowIndex = secondRowIndex;
        }

        public void ApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < target.Width; i++)
            {
                T temp = target[FirstRowIndex, i];
                target[FirstRowIndex, i] = target[SecondRowIndex, i];
                target[SecondRowIndex, i] = temp;
            }
        }

        public void InverseApplyTo(Matrix<T> target)
        {
            ApplyTo(target);
        }
    }

    public class SwapColumns<T> : MatrixOperation<T> where T : Field<T>, new()
    {
        public int FirstColumnIndex { get; set; }
        public int SecondColumnIndex { get; set; }

        public SwapColumns(int firstColumnIndex, int secondColumnIndex)
        {
            FirstColumnIndex = firstColumnIndex;
            SecondColumnIndex = secondColumnIndex;
        }

        public void ApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < target.Height; i++)
            {
                T temp = target[i, FirstColumnIndex];
                target[i, FirstColumnIndex] = target[i, SecondColumnIndex];
                target[i, SecondColumnIndex] = temp;
            }
        }

        public void InverseApplyTo(Matrix<T> target)
        {
            ApplyTo(target);
        }
    }

    public class MultiplyRowByScalar<T> : MatrixOperation<T> where T : Field<T>, new()
    {
        public int RowIndex { get; set; }
        public T Scalar { get; set; }

        public MultiplyRowByScalar(int rowIndex, T scalar)
        {
            RowIndex = rowIndex;
            Scalar = scalar;
        }

        public void ApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < target.Width; i++)
            {
                target[RowIndex, i] = target[RowIndex, i].Multiply(Scalar);
            }
        }

        public void InverseApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < target.Width; i++)
            {
                target[RowIndex, i] = target[RowIndex, i].Multiply(Scalar.MultiplicativeInverse);
            }
        }
    }

    public class AddRowMultiple<T> : MatrixOperation<T> where T : Field<T>, new()
    {
        public int BaseRowIndex { get; set; }
        public int TargetRowIndex { get; set; }
        public T Scalar { get; set; }

        public AddRowMultiple(int baseRowIndex, int targetRowIndex, T scalar)
        {
            BaseRowIndex = baseRowIndex;
            TargetRowIndex = targetRowIndex;
            Scalar = scalar;
        }

        public void ApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < target.Width; i++)
            {
                target[TargetRowIndex, i] = target[TargetRowIndex, i].Add(target[BaseRowIndex, i].Multiply(Scalar));
            }
        }

        public void InverseApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < target.Width; i++)
            {
                target[TargetRowIndex, i] = target[TargetRowIndex, i].Subtract(target[BaseRowIndex, i].Multiply(Scalar));
            }
        }
    }

    public class RowPermutation<T> : MatrixOperation<T> where T : Field<T>, new()
    {
        public List<int> IndexList;

        public int Signature
        {
            get
            {
                int sign = 1;
                bool[] visited = new bool[IndexList.Count];

                for (int i = 0; i < IndexList.Count; i++)
                {
                    if (!visited[i])
                    {
                        int next = i;
                        bool isCycleLenEven = true;

                        while (!visited[next])
                        {
                            visited[next] = true;
                            isCycleLenEven = !isCycleLenEven;
                            next = IndexList[next];
                        }

                        if (isCycleLenEven)
                        {
                            sign *= -1;
                        }
                    }
                }

                return sign;
            }
        }

        public RowPermutation(params int[] indices)
        {
            IndexList = new List<int>(indices.Length);
            IndexList.AddRange(indices);
        }

        public Matrix<T> ToMatrix()
        {
            Matrix<T> mat = new Matrix<T>(IndexList.Count, IndexList.Count);

            for (int i = 0; i < IndexList.Count; i++)
            {
                mat[i, IndexList[i]] = new T().One;
            }

            return mat;
        }

        public void ApplyTo(Matrix<T> target)
        {
            T[,] mat = new T[target.Rows, target.Columns];

            for (int i = 0; i < IndexList.Count; i++)
            {
                for (int j = 0; j < target.Width; j++)
                {
                    mat[IndexList[i], j] = target[i, j];
                }
            }

            for (int i = 0; i < target.Rows; i++)
            {
                for (int j = 0; j < target.Columns; j++)
                {
                    target[i, j] = mat[i, j];
                }
            }
        }

        public void InverseApplyTo(Matrix<T> target)
        {
            for (int i = IndexList.Count - 1; i >= 0; i++)
            {
                new SwapRows<T>(i, IndexList[i]).ApplyTo(target);
            }
        }
    }

    public class ColumnPermutation<T> : MatrixOperation<T> where T : Field<T>, new()
    {
        public List<int> IndexList { get; set; }

        public ColumnPermutation(params int[] indices)
        {
            IndexList = new List<int>();

            for (int i = 0; i < indices.Length; i++)
            {
                IndexList.Add(indices[i]);
            }
        }

        public void ApplyTo(Matrix<T> target)
        {
            for (int i = 0; i < IndexList.Count; i++)
            {
                new SwapColumns<T>(i, IndexList[i]).ApplyTo(target);
            }
        }

        public void InverseApplyTo(Matrix<T> target)
        {
            for (int i = IndexList.Count - 1; i >= 0; i++)
            {
                new SwapColumns<T>(i, IndexList[i]).ApplyTo(target);
            }
        }
    }

}
