using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.LinearAlgebra.MatrixOperations
{
    public interface MatrixOpertaion<T> where T : Field<T>, new()
    {
        void ApplyTo(Matrix<T> target);
    }

    public class SwapRows<T> : MatrixOpertaion<T> where T : Field<T>, new()
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
    }

    public class MultiplyRowByScalar<T> : MatrixOpertaion<T> where T : Field<T>, new()
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
    }

    public class AddRowMultiple<T> : MatrixOpertaion<T> where T : Field<T>, new()
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
    }

}
