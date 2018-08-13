using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedMath.LinearAlgebra;
using System.Diagnostics;
using RedMath.Structures;
using RedMath.Utils;

namespace RedMathTests
{
    [TestClass]
    public class MatrixTests
    {
        public static Matrix<Real> TestMatrix1 = new Matrix<Real>
        (
            new Real[,]
            {
                { 1,  2,   7, 3 },
                { 3, -1,   4, 4 },
                { 1, -3, -10, 9 }
            }
        );

        public static Matrix<Real> TestMatrix2 = new Matrix<Real>
        (
            new Real[,]
            {
                { 1,  2,   7 },
                { 3, -1,   4 },
                { 5,  4, -10 }
            }
        );

        public static Matrix<GF5> H = new Matrix<GF5>
        (
            new GF5[,]
            {
                { new GF5(1), new GF5(2), new GF5(4), new GF5(2), new GF5(4), new GF5(2), new GF5(1) },
                { new GF5(4), new GF5(2), new GF5(2), new GF5(3), new GF5(3), new GF5(4), new GF5(2) },
                { new GF5(1), new GF5(3), new GF5(3), new GF5(3), new GF5(2), new GF5(2), new GF5(4) }
            }
        );

        [TestMethod]
        public void Height()
        {
            Assert.AreEqual(3, TestMatrix1.Height);
        }

        [TestMethod]
        public void Width()
        {
            Assert.AreEqual(4, TestMatrix1.Width);
        }

        [TestMethod]
        public void Rows()
        {
            Assert.AreEqual(3, TestMatrix1.Rows);
        }

        [TestMethod]
        public void Columns()
        {
            Assert.AreEqual(4, TestMatrix1.Columns);
        }

        [TestMethod]
        public void IsRowMatrix()
        {
            Assert.AreEqual(false, TestMatrix1.IsRowMatrix);
        }

        [TestMethod]
        public void IsColumnMatrix()
        {
            Assert.AreEqual(false, TestMatrix1.IsColumnMatrix);
        }

        [TestMethod]
        public void IsSquareMatrix()
        {
            Assert.AreEqual(false, TestMatrix1.IsSquareMatrix);
        }

        [TestMethod]
        public void IsIdentity()
        {
            Assert.AreEqual(false, TestMatrix1.IsIdentity);
        }

        [TestMethod]
        public void IsLowerTriangular()
        {
            Assert.AreEqual(false, TestMatrix1.IsLowerTriangular);
        }

        [TestMethod]
        public void IsUpperTriangular()
        {
            Assert.AreEqual(false, TestMatrix1.IsUpperTriangular);
        }

        [TestMethod]
        public void MainDiagonal()
        {
            Real[] diag = TestMatrix1.MainDiagonal;
            Real[] expected = new Real[] { 1, -1, -10 };

            for (int i = 0; i < diag.Length; i++)
            {
                Assert.AreEqual(expected[i], diag[i]);
            }
        }

        [TestMethod]
        public void AntiDiagonal()
        {
            Real[] expected = new Real[] { 1, -1, 7 };

            Assert.AreEqual(expected[0], TestMatrix1.AntiDiagonal[0]);
            Assert.AreEqual(expected[1], TestMatrix1.AntiDiagonal[1]);
            Assert.AreEqual(expected[2], TestMatrix1.AntiDiagonal[2]);
        }

        [TestMethod]
        public void Decomposition()
        {
            Matrix<Real> reconstruction = TestMatrix1.Decomposition.Permutation.ToMatrix() * TestMatrix1.Decomposition.LowerMatrix * TestMatrix1.Decomposition.UpperMatrix;
            double diffSum = 0;
            foreach (var val in ((Real[,])(TestMatrix1 - reconstruction))) { diffSum += val; }
            Assert.IsTrue(diffSum < 1e-12);
            //Assert.AreEqual(TestMatrix1, reconstruction, "Average Difference: " + diffSum);
        }

        [TestMethod]
        public void Determinant()
        {
            Assert.AreEqual(213, Math.Floor(TestMatrix2.Determinant));
        }

        [TestMethod]
        public void EchelonForm()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1, 2, 7       },
                    { 0, 1, 82D/35D },
                    { 0, 0, 1       }
                }
            );

            double diffSum = 0;
            foreach (var val in ((Real[,])(TestMatrix2.EchelonForm - expected))) { diffSum += val; }
            Assert.IsTrue(diffSum < 1);
            //Assert.AreEqual(expected, TestMatrix2.EchelonForm);
        }

        [TestMethod]
        public void ReducedEchelonForm()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                }
            );

            Assert.AreEqual(expected, TestMatrix2.ReducedEchelonForm);
        }

        [TestMethod]
        public void CofactorMatrix()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { -6,   50,  17 },
                    {  48, -45,  6  },
                    {  15,  17, -7  }
                }
            );

            var cofactorMat = TestMatrix2.CofactorMatrix;

            for (int i = 0; i < cofactorMat.Rows; i++)
            {
                for (int j = 0; j < cofactorMat.Columns; j++)
                {
                    cofactorMat[i, j] = Math.Round(cofactorMat[i, j]);
                }
            }

            Assert.AreEqual(expected, cofactorMat);
        }

        [TestMethod]
        public void InverseMatrix()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                }
            );

            double diffSum = 0;
            foreach (var val in ((Real[,])(TestMatrix2.Inverse * TestMatrix2 - expected))) { diffSum += val; }
            Assert.IsTrue(diffSum < 1e-12);
            //Assert.AreEqual(expected, TestMatrix2.Inverse * TestMatrix2);
        }

        [TestMethod]
        public void Rank()
        {
            Assert.AreEqual(3, TestMatrix1.Rank);
        }

        [TestMethod]
        public void IsFullRank()
        {
            Assert.AreEqual(false, TestMatrix1.IsFullRank);
        }

        [TestMethod]
        public void Transposition()
        {
            Assert.AreEqual(TestMatrix1, TestMatrix1.Transposition.Transposition);
        }

        [TestMethod]
        public void SubMatrix()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1, 7, 3 },
                    { 3, 4, 4 }
                }
            );

            Assert.AreEqual(expected, TestMatrix1.SubMatrix(2, 1));
        }

        [TestMethod]
        public void Resize()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1,  2 },
                    { 3, -1 },
                }
            );

            Matrix<Real> testMatrix1 = new Matrix<Real>(TestMatrix1);
            testMatrix1.Resize(2, 2);

            Assert.AreEqual(expected, testMatrix1);
        }

        [TestMethod]
        public void Transpose()
        {
            Matrix<Real> testMatrix1 = new Matrix<Real>(TestMatrix1);
            testMatrix1.Transpose();
            testMatrix1.Transpose();

            Assert.AreEqual(TestMatrix1, testMatrix1);
        }

        [TestMethod]
        public void GetRowVector()
        {
            Assert.AreEqual(new Vector<Real>(3, -1, 4, 4), TestMatrix1.GetRowVector(1));
        }

        [TestMethod]
        public void GetColumnVector()
        {
            Assert.AreEqual(new Vector<Real>(3, 4, 9), TestMatrix1.GetColumnVector(3));
        }

        [TestMethod]
        public void AppendRowVector()
        {
            Matrix<Real> testMatrix1 = new Matrix<Real>(TestMatrix1);
            testMatrix1.AppendRowVector(new Vector<Real>(2, 3));

            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1,  2,   7, 3 },
                    { 3, -1,   4, 4 },
                    { 1, -3, -10, 9 },
                    { 2,  3,   0, 0 }
                }
            );

            Assert.AreEqual(expected, testMatrix1);
        }

        [TestMethod]
        public void InsertRowVector()
        {
            Matrix<Real> testMatrix1 = new Matrix<Real>(TestMatrix1);
            testMatrix1.InsertRowVector(new Vector<Real>(2, 3), 1);

            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1,  2,   7, 3 },
                    { 2,  3,   0, 0 },
                    { 3, -1,   4, 4 },
                    { 1, -3, -10, 9 }
                }
            );

            Assert.AreEqual(expected, testMatrix1);
        }

        [TestMethod]
        public void AppendColumnVector()
        {
            Matrix<Real> testMatrix1 = new Matrix<Real>(TestMatrix1);
            testMatrix1.AppendColumnVector(new Vector<Real>(2, 3));

            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1,  2,   7, 3, 2 },
                    { 3, -1,   4, 4, 3 },
                    { 1, -3, -10, 9, 0 },
                }
            );

            Assert.AreEqual(expected, testMatrix1);
        }

        [TestMethod]
        public void InsertColumnVector()
        {
            Matrix<Real> testMatrix1 = new Matrix<Real>(TestMatrix1);
            testMatrix1.InsertColumnVector(new Vector<Real>(2, 3), 1);

            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 1, 2,  2,   7, 3 },
                    { 3, 3, -1,   4, 4 },
                    { 1, 0, -3, -10, 9 }
                }
            );

            Assert.AreEqual(expected, testMatrix1);
        }

        [TestMethod]
        public void Minor()
        {
            Assert.AreEqual(-7, TestMatrix2.Minor(2, 2));
        }

        [TestMethod]
        public void Cofactor()
        {
            Assert.AreEqual(-7, TestMatrix2.Cofactor(2, 2));
        }

        [TestMethod]
        public void MatrixMatrixMultiplication()
        {
            Matrix<Real> expected = new Matrix<Real>
            (
                new Real[,]
                {
                    { 14, -21, - 55,  74 },
                    { 4,  - 5, - 23,  41 },
                    { 7,   36,  151, -59 }
                }
            );

            Assert.AreEqual(expected, TestMatrix2 * TestMatrix1);
        }

        [TestMethod]
        public void MatrixVectorMultiplication()
        {
            Vector<Real> testVector = new Vector<Real>(5, 2, 9);
            Vector<Real> expected = new Vector<Real>(72, 49, -57);

            Assert.AreEqual(expected, TestMatrix2 * testVector);
        }

        [TestMethod]
        public void VectorMatrixMultiplication()
        {
            Vector<Real> testVector = new Vector<Real>(3, 6, 4);
            Vector<Real> expected = new Vector<Real>(41, 16, 5);

            Assert.AreEqual(expected, testVector * TestMatrix2);
        }

        [TestMethod]
        public void GF5Test()
        {
            Debug.WriteLine(H.ReducedEchelonForm);
        }
    }
}
