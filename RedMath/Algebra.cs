using RedMath.Structures;
using System;

namespace RedMath
{
    public static class Algebra
    {
        public static ulong Factorial(ulong x)
        {
            if (x == 1 || x == 0)
            {
                return 1;
            }
            else
            {
                return x * Factorial(x - 1);
            }
        }

        public static Complex ComplexExp(Complex z)
        {
            double ePow;

            if (z.Real == 0)
            {
                ePow = 1;
            }
            else if (z.Real == 1)
            {
                ePow = Math.E;
            }
            else
            {
                ePow = Math.Exp(z.Real);
            }

            return new Complex(ePow * Math.Cos(z.Imaginary), ePow * Math.Sin(z.Imaginary));
        }

        public static double AGM(double x, double y)
        {
            double a = (x + y) / 2;
            double g = Math.Sqrt(x * y);

            while (Math.Abs(a - g) > 0.001)
            {
                a = (a + g) / 2;
                g = Math.Sqrt(a * g);
            }

            return a;
        }

        public static bool IsPowerOfTwo(ulong x)
        {
            return (x & (x - 1)) == 0;
        }

        public static int NextGreaterPowerOfTwo(int x)
        {
            x--;
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return (x + 1);
        }

        public static double NthRoot(double a, double n)
        {
            return Math.Pow(a, 1 / n);
        }

        
        public static T IntPower<T>(T b, int n) where T : Field<T>, new()
        {
            T prod = Field<T>.One;

            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    prod *= b;
                }
            }

            return prod;
        }

        public static long IntPower(int b, int n)
        {
            long prod = 1;

            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    prod *= b;
                }
            }

            return prod;
        }

        public static long GreatestCommonDivisor(long left, long right)
        {
            left = Math.Abs(left);
            right = Math.Abs(right);

            if (right > left)
            {
                long temp = left;
                left = right;
                right = temp;
            }

            while (right != 0)
            {
                long temp = right;
                right = left % right;
                left = temp;
            }

            return left;
        }
    }
}
