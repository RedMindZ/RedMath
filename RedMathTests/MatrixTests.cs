using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedMath.LinearAlgebra;
using System.Diagnostics;
using RedMath.Structures;

namespace RedMathTests
{
    [TestClass]
    public class MatrixTests
    {
        Matrix<Real> mat = new Matrix<Real>
                (
                new Real[,]
                {
                    { 0, 1, 2, 1 },
                    { 3, 3, 3, 5 },
                    { -3, 0, 3, -2 }
                }
                );

        Matrix<Real> mat2 = new Matrix<Real>
                (
                new Real[,]
                {
                    { 0, 0 },
                    { 0, 1 },
                }
                );

        Matrix<Real> equationMat = new Matrix<Real>
                (
                new Real[,]
                {
                    { 1, 2, 1, 3, -5 },
                    { 1, 3, 2, 5, -2 },
                    { 3, 7, 5, 14, -4 }
                }
                );

        Matrix<Complex> complexMat = new Matrix<Complex>
                (
                new Complex[,]
                {
                    { new Complex(2, -1), new Complex(3, 0), new Complex(0, 1) },
                    { new Complex(0, 2), new Complex(1, 2), new Complex(-1, 0) },
                    { new Complex(1, -2), new Complex(0, -1), new Complex(1, 1) }
                }
                );

        Matrix<Complex> PMat = new Matrix<Complex>
            (
            new Complex[,]
            {
                { new Complex(0, 1), new Complex(1, 0) },
                { new Complex(1, 0), new Complex(0, 1) }
            }
            );

        [TestMethod]
        public void DecompositionTest()
        {


            Debug.WriteLine(mat);
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.EchelonForm);
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.ReducedEchelonForm);
            Debug.WriteLine("");

            Debug.WriteLine("L:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.Decomposition.Item1);
            Debug.WriteLine("");
            Debug.WriteLine("U:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.Decomposition.Item2);
            Debug.WriteLine("");
            Debug.WriteLine("P:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.Decomposition.Item3.ToMatrix());
            Debug.WriteLine("");
            Debug.WriteLine("P * L * U:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.Decomposition.Item3.ToMatrix() * mat.Decomposition.Item1 * mat.Decomposition.Item2);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void DeterminantTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(mat);
            Debug.WriteLine("");
            Debug.WriteLine("Detrminant:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.Determinant);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void ComplexDeterminantTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(complexMat);
            Debug.WriteLine("");
            Debug.WriteLine("Detrminant:");
            Debug.WriteLine("--->");
            Debug.WriteLine(complexMat.Determinant);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void InverseTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(mat);
            Debug.WriteLine("");
            Debug.WriteLine("Inverse:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.Inverse);
            Debug.WriteLine("");
            Debug.WriteLine("Matrix * Inverse:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat * mat.Inverse);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void ComplexInverseTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(PMat);
            Debug.WriteLine("");
            Debug.WriteLine("Inverse:");
            Debug.WriteLine("--->");
            Debug.WriteLine(PMat.Inverse);
            Debug.WriteLine("");
            Debug.WriteLine("Matrix * Inverse:");
            Debug.WriteLine("--->");
            Debug.WriteLine(PMat * PMat.Inverse);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void ReductionTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(mat);
            Debug.WriteLine("");
            Debug.WriteLine("Echelon Form:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.EchelonForm);
            Debug.WriteLine("");
            Debug.WriteLine("Reduced Echelon Form:");
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.ReducedEchelonForm);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void EquationTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(equationMat);
            Debug.WriteLine("");
            Debug.WriteLine("Echelon Form:");
            Debug.WriteLine("--->");
            Debug.WriteLine(equationMat.EchelonForm);
            Debug.WriteLine("");
            Debug.WriteLine("Reduced Echelon Form:");
            Debug.WriteLine("--->");
            Debug.WriteLine(equationMat.ReducedEchelonForm);
            Debug.WriteLine("");
        }

        [TestMethod]
        public void TrasformationTest()
        {
            Debug.WriteLine("Matrix:");
            Debug.WriteLine(MatrixTransformations.CreateLookAtMatrix(new Vector<Real>(1, 1, 1, 1), new Vector<Real>(2, 3, 2, 6), new Vector<Real>(0, 1, 0, 3), new Vector<Real>(1, 0, 3, 2)));
            Debug.WriteLine("");
        }

        [TestMethod]
        public void VectorHomTest()
        {
            Vector<Real> vec = new Vector<Real>(1, 2, 3);

            Debug.WriteLine("Vector:");
            Debug.WriteLine(vec);
            Debug.WriteLine("Vector Hom:");
            Debug.WriteLine(vec.HomogeneousCoordinates);
        }
    }
}
