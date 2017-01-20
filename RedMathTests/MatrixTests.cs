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
                    { 1, 2, 4, 8, 16 },
                    { 32, 64, 128, 256, 512 },
                    { 1024, 2048, 4096, 8192, 16384 }
                }
                );

            Debug.WriteLine(mat);
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.computeEchelonForm());
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.ReducedEchelonForm);
            Debug.WriteLine("");

            mat[0, 0] = 0;

            Debug.WriteLine(mat);
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.computeEchelonForm());
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.ReducedEchelonForm);
            Debug.WriteLine("");

            mat[1, 0] = 0;

            Debug.WriteLine(mat);
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.computeEchelonForm());
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.ReducedEchelonForm);
            Debug.WriteLine("");

            mat[0, 0] = 1;
            mat[1, 0] = 32;
            mat[2, 0] = 1024;

            mat[1, 0] = 0;
            mat[1, 1] = 0;
            mat[1, 2] = 0;
            mat[1, 3] = 0;
            mat[1, 4] = 0;
            mat[2, 0] = 40;

            Debug.WriteLine(mat);
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.computeEchelonForm());
            Debug.WriteLine("--->");
            Debug.WriteLine(mat.ReducedEchelonForm);
            Debug.WriteLine("");
        }
    }
}
