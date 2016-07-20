using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedMath.LinearAlgebra;
using System.Diagnostics;

namespace RedMathTests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void DecompositionTest()
        {
            Matrix m = new Matrix
            (
                new double[,]
                {
                    { 1, 0, 2 },
                    { 0, 1, 1/4.0 },
                    { -3, 0, 2 },
                }
            );

            Assert.AreEqual(m, m.Decomposition.Item1 * m.Decomposition.Item2 * m.Decomposition.Item3);
        }

        [TestMethod]
        public void InversionTest()
        {
            Matrix m = new Matrix
            (
                new double[,]
                {
                    { 1, 4, 0 },
                    { 0, 0, 1 },
                    { 0, 1, 2 },
                }
            );

            Console.WriteLine(m);
            Console.WriteLine();
            Console.WriteLine(m.Inverse);

            Assert.AreEqual(new Matrix(3, 3), m.Inverse * m);
        }

        [TestMethod]
        public void RotationTest()
        {
            Matrix target = new Matrix
            (
                new double[,]
                {
                    { 0, -1, 0 },
                    { 1,  0, 0 },
                    { 0,  0, 1 }
                }
            );

            Matrix ax = new Matrix(new double[,] { { 0 }, { 0 }, { 1 } });

            Matrix res = MatrixTransformations.Rotation(ax, Math.PI);

            Assert.AreEqual(target, res);
        }

        public static Tuple<Matrix, Matrix, Matrix> LUPDecomposition(Matrix m)
        {
            int n = m.Height;
            /*
            * pi represents the permutation matrix.  We implement it as an array
            * whose value indicates which column the 1 would appear.  We use it to avoid 
            * dividing by zero or small numbers.
            * */
            Vector perm = new Vector(n);
            double max = 0;
            int maxIndex = 0;
            double temp = 0;

            Matrix result = new Matrix(m);

            Matrix L;
            Matrix U;
            Matrix P = new Matrix(n, n);

            if (m.Width < m.Height)
            {
                U = new Matrix(m.Columns, m.Columns);
                L = new Matrix(m.Rows, m.Columns);

                for (int i = 0; i < L.Width; i++)
                {
                    L[i, i] = 1;
                }
            }
            else if (m.Width > m.Height)
            {
                U = new Matrix(m.Rows, m.Columns);
                L = new Matrix(m.Rows, m.Rows);

                for (int i = 0; i < L.Height; i++)
                {
                    L[i, i] = 1;
                }
            }
            else
            {
                U = new Matrix(m.Rows, m.Columns);
                L = new Matrix(m.Rows, m.Columns);

                for (int i = 0; i < L.Height; i++)
                {
                    L[i, i] = 1;
                }
            }

            //Initialize the permutation matrix, will be the identity matrix
            for (int i = 0; i < n; i++)
            {
                perm[i] = i;
            }

            for (int i = 0; i < n; i++)
            {
                /*
                * In finding the permutation matrix p that avoids dividing by zero
                * we take a slightly different approach.  For numerical stability
                * We find the element with the largest 
                * absolute value of those in the current first column (column k).  If all elements in
                * the current first column are zero then the matrix is singluar and throw an
                * error.
                * */
                max = 0;
                for (int j = i; j < n; j++)
                {
                    if (Math.Abs(result[j, i]) > max)
                    {
                        max = Math.Abs(result[j, i]);
                        maxIndex = j;
                    }
                }
                if (max == 0)
                {
                    return null;
                }
                /*
                * These lines update the pivot array (which represents the pivot matrix)
                * by exchanging pi[k] and pi[kp].
                * */
                temp = perm[i];
                perm[i] = perm[maxIndex];
                perm[maxIndex] = temp;

                /*
                * Exchange rows k and kpi as determined by the pivot
                * */
                for (int j = 0; j < n; j++)
                {
                    temp = result[i,j];
                    result[i, j] = result[maxIndex, j];
                    result[maxIndex, j] = temp;
                }

                /*
                    * Compute the Schur complement
                    * */
                for (int j = i + 1; j < n; j++)
                {
                    result[j, i] /= result[i, i];
                    for (int k = i + 1; k < n; k++)
                    {
                        result[j, k] -= result[j, i] * result[i, k];
                    }
                }
            }

            for (int i = 0; i < L.Height; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    L[i, j] = result[i, j];
                }
            }

            for (int i = 0; i < U.Height; i++)
            {
                for (int j = i; j < U.Width; j++)
                {
                    U[i, j] = result[i, j];
                }
            }

            for (int i = 0; i < P.Height; i++)
            {
                P[i, i] = 0;

                P[i, (int)perm[i]] = 1;
            }

            return new Tuple<Matrix, Matrix, Matrix>(P, L, U);
        }
    }
}
