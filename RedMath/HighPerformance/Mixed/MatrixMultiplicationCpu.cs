using System;
using System.Threading.Tasks;

using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMath.HighPerformance.Mixed
{
    public static partial class MatrixMultiplication
    {
        public static Matrix<T> ParallelMultiply<T>(Matrix<T> left, Matrix<T> right) where T : Field<T>, new()
        {
            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("Matrices of incompatible sizes can't be multiplied.");
            }

            T[,] temp = new T[left.Height, right.Width];
            T fieldZero = new T().Zero;

            Parallel.For(0, left.Height * right.Width, (index) =>
            {
                int rowIndex = index / left.Height;
                int colIndex = index % right.Width;

                temp[rowIndex, colIndex] = fieldZero.Clone();

                for (int k = 0; k < left.Width; k++)
                {
                    temp[rowIndex, colIndex] += left[rowIndex, k] * right[k, colIndex];
                }
            });

            return new Matrix<T>(temp);
        }

        
    }
}
