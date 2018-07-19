using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using RedMath;
using RedMath.Utils;
using RedMath.Structures;
using RedMath.LinearAlgebra;
using RedMath.LinearAlgebra.MatrixOperations;
using RedMath.Structures.Expression;
using RedMath.HighPerformance;
using System.Threading;

namespace ExecutionTest
{
    class Program
    {
        private static Matrix<Real> m1 = new Matrix<Real>(new Real[,] { { 2, 4 }, { 6, 8 } });
        private static Matrix<Complex> m2 = new Matrix<Complex>(new Complex[,] { { (Real)1, (Real)2 }, { (Real)3, (Real)4 } });



        static void Main(string[] args)
        {

            TestMatMul();

            Console.WriteLine("The test is over. Press any key to continue...");
            Console.ReadKey();
        }

        private static void TestMatMul()
        {
            int testCount = 10000;
            Stopwatch sw = new Stopwatch();

            Rational[,] matData = new Rational[3, 3];
            matData.AssignAll(ind => new Rational(ind[0] * matData.GetLength(0) + ind[1]));
            Matrix<Rational> mat1 = new Matrix<Rational>(matData);
            Matrix<Rational> mat2 = new Matrix<Rational>(mat1.Transposition);

            Console.WriteLine("Multiplying matrices:");
            Console.WriteLine(mat1);
            Console.WriteLine("And:");
            Console.WriteLine(mat2);
            Console.WriteLine("For " + testCount + " times...");

            sw.Start();

            Matrix<Rational> res = null;
            for (int i = 0; i < testCount; i++)
            {
                res = mat1 * mat2;
            }

            sw.Stop();

            Console.WriteLine(res);
            Console.WriteLine("Measured time: " + sw.Elapsed.TotalMilliseconds + "ms");
            Console.WriteLine("Average time per operation: " + sw.Elapsed.TotalMilliseconds / testCount + "ms");
            Console.WriteLine();


            Console.WriteLine("Now testing with ParallelMultiply:");

            sw.Restart();

            for (int i = 0; i < testCount; i++)
            {
                res = MatrixMultiplication.CpuParallelMultiply(mat1, mat2);
            }

            sw.Stop();

            Console.WriteLine(res);
            Console.WriteLine("Measured time: " + sw.Elapsed.TotalMilliseconds + "ms");
            Console.WriteLine("Average time per operation: " + sw.Elapsed.TotalMilliseconds / testCount + "ms");
            Console.WriteLine();

            MatrixMultiplication.GpuMultiply<Rational, GpuRational>(mat1, mat2); // 'Warmup' the gpu

            Console.WriteLine("Now testing with GPUMultiply:");

            sw.Restart();

            for (int i = 0; i < testCount; i++)
            {
                res = MatrixMultiplication.GpuMultiply<Rational, GpuRational>(mat1, mat2);
            }

            sw.Stop();

            Console.WriteLine(res);
            Console.WriteLine("Measured time: " + sw.Elapsed.TotalMilliseconds + "ms");
            Console.WriteLine("Average time per operation: " + sw.Elapsed.TotalMilliseconds / testCount + "ms");
            Console.WriteLine();
        }

        private static void TestAliasing()
        {
            Real[] arr = m1.AntiDiagonal;
            arr[0].Value = 10;
            Console.WriteLine(m1);
        }

        private static void TestProperties(bool print = false)
        {
            /*
             "determinant"
             "echelonFormOperations"
             "reducedEchelonFormOperations" 
             "echelonForm"
             "reducedEchelonForm"
             "decomposition"
             "identity"
             "cofactorMatrix"
            */

            Stopwatch sw = new Stopwatch();

            sw.Start();
            var det = m1.Determinant;
            var efro = m1.EchelonFormReductionOperations;
            var refro = m1.ReducedEchelonFormReductionOperations;
            var ef = m1.EchelonForm;
            var reef = m1.ReducedEchelonForm;
            var dec = m1.Decomposition;
            var ii = m1.IsIdentity;
            //var cm = m1.CofactorMatrix;
            sw.Stop();

            if (print)
            {
                Console.WriteLine(det);
                Console.WriteLine(efro);
                Console.WriteLine(refro);
                Console.WriteLine(ef);
                Console.WriteLine(reef);
                Console.WriteLine(dec);
                Console.WriteLine(ii);
                //Console.WriteLine(cm); 
            }

            Console.WriteLine("Measured time: " + sw.ElapsedMilliseconds);
            Console.WriteLine();
        }

        private static void TestEquality()
        {
            bool res = false;

            Console.WriteLine("M1:");
            Console.WriteLine(m1);
            Console.WriteLine("");

            Console.WriteLine("M2:");
            Console.WriteLine(m2);
            Console.WriteLine("");

            //res = m1 == m2;
            Console.WriteLine("M1 == M2");
            Console.WriteLine(res);

            res = m1.Equals(m2);
            Console.WriteLine("M1.Equals(M2)");
            Console.WriteLine(res);
        }

        private static void TestCache()
        {
            Cache<string> cache = new Cache<string>();

            cache.AddCacheEntry("Det", 5, false, (Matrix<Real> mat) => mat.Determinant);
            cache.AddCacheEntry("Trans", null, false, (Matrix<Real> mat) => mat.Transposition);
            cache.AddCacheEntry("Inverse", null, false, (Matrix<Real> mat) => mat.Inverse);

            Console.WriteLine("Det:\n" + cache.RetrieveValue<Real, Matrix<Real>>("Det", m1));
            Console.WriteLine("Trans:\n" + cache.RetrieveValue<Matrix<Real>, Matrix<Real>>("Trans", m1));
            Console.WriteLine("Inverse:\n" + cache.RetrieveValue<Matrix<Real>, Matrix<Real>>("Inverse", m1));

            cache.SetAllUpdateStates(true);

            Console.WriteLine("Det:\n" + cache.RetrieveValue<Real, Matrix<Real>>("Det", m1));
            Console.WriteLine("Trans:\n" + cache.RetrieveValue<Matrix<Real>, Matrix<Real>>("Trans", m1));
            Console.WriteLine("Inverse:\n" + cache.RetrieveValue<Matrix<Real>, Matrix<Real>>("Inverse", m1));
        }

        private static void TestExpTree()
        {
            AddNode<Real> root = new AddNode<Real>();

            MultiplyNode<Real> c1 = new MultiplyNode<Real>();
            SubtractNode<Real> c2 = new SubtractNode<Real>();
            DivideNode<Real> c3 = new DivideNode<Real>();

            root.LeftChild = c1;
            root.RightChild = c2;

            c1.LeftChild = new ConstantNode<Real>(2);
            c1.RightChild = c3;

            c2.LeftChild = new ConstantNode<Real>(7);
            c2.RightChild = new ConstantNode<Real>(4);

            c3.LeftChild = new ConstantNode<Real>(8);
            c3.RightChild = new ConstantNode<Real>(16);

            Console.WriteLine(root.ToString() + " = " + root.Evaluate());
        }

        private static void LUPTest()
        {
            m1[0, 0] = 0;

            LUPDecomposition<Real> decomp = new LUPDecomposition<Real>(m1);

            Console.WriteLine("Lower:");
            Console.WriteLine(decomp.LowerMatrix);
            Console.WriteLine();
            Console.WriteLine("Upper:");
            Console.WriteLine(decomp.UpperMatrix);
            Console.WriteLine();
            Console.WriteLine("Permutation:");
            Console.WriteLine(decomp.Permutation.ToMatrix());
            Console.WriteLine("P*L*U:");
            Console.WriteLine(decomp.Permutation.ToMatrix() * decomp.LowerMatrix * decomp.UpperMatrix);
        }

        private static void PermutationParityTest()
        {
            RowPermutation<Real> perm = new RowPermutation<Real>(2, 4, 1, 3, 0);

            Console.WriteLine("Parity:");
            Console.WriteLine(perm.Signature);
        }
    }
}
