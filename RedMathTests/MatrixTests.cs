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
        [TestMethod]
        public void DecompositionTest()
        {
            Matrix<Real> mat = new Matrix<Real>
                (
                new Real[,]
                {
                    { 1, 2, -3, 1 },
                    { 2, 4, 0, 7, },
                    { -1, 3, 2, 0 }
                }
                );

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
    }
}
