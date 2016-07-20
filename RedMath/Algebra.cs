using System;

namespace RedMath
{
    public static class Algebra
    {
        public const double PI = 3.1415926535897;
        public const double E = 2.7182818284590;

        public static double Power(double b, double x)
        {
            /*if (b >= 0)
            {
                return Exponent(x * NaturalLogarithm(b));
            }
            else
            {
                if (x % 2 == 0)
                {
                    return Exponent(x * NaturalLogarithm(-b));
                }
                else
                {
                    return -Exponent(x * NaturalLogarithm(-b));
                }
            }*/

            return Math.Pow(b, x);
        }

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

        public static double Exponent(double x)
        {
            if (x == 0)
                return 1;

            if (x == 1)
                return E;
            /*double sum = 0;

            for (int n = 0; n < 24; n++)
            {
                sum += IntPower(x, n) / Factorial((uint)n);
            }

            return sum;*/
            return Math.Exp(x);
        }

        public static Complex Exponent(Complex z)
        {
            double ePow;

            if (z.Real == 0)
            {
                ePow = 1;
            }
            else if (z.Real == 1)
            {
                ePow = E;
            }
            else
            {
                ePow = Exponent(z.Real);
            }

            return new Complex(ePow * Trigonometry.Cos(z.Imaginary), ePow * Trigonometry.Sin(z.Imaginary));
        }

        public static double IntExponent(int x)
        {
            return IntPower(E, x);
        }

        public static double NaturalLogarithm(double x)
        {
            if (x == 0)
                return Double.NegativeInfinity;

            if (x == 1)
                return 0;

            return Math.Log(x);

            /*double sum1 = 0;
            double sum2 = 0;

            int i = 0;
            double xi = 1;

            double n = 1;
            double deltaX = 1;

            double b = 2;

            if (t > 1)
            {
                while (xi < t)
                {
                    xi = SquareRoot(IntPower(b, ++i));
                }
                i--;
                xi = SquareRoot(IntPower(b, i));

                for (int j = 0; j < i + 1; j++)
                {
                    xi = SquareRoot(IntPower(b, j));
                    double xi1 = SquareRoot(IntPower(b, j + 1));

                    if (j < i)
                    {
                        n = Ceiling(1 / IntPower(xi1 - xi, 10)) + t;
                        deltaX = (xi1 - xi) / n; 
                    }
                    else
                    {
                        n = Ceiling(1 / IntPower(t - xi, 10)) + t;
                        deltaX = (t - xi) / n;
                    }

                    for (int k = 1; k < n + 1; k++)
                    {
                        double top = deltaX;
                        double bottom = (deltaX * k + xi);
                        sum1 += top / bottom;
                    }

                    sum2 += sum1;
                    sum1 = 0;
                }
            }

            return sum2;*/

            /*double pow = 0;
            double dist = 0;

            while (IntExponent((int)pow) < t)
            {
                pow++;
            }

            int i = 0;

            do
            {
                dist = Exponent(pow) - t;

                if (dist > 0)
                {
                    pow -= 1 / IntPower(2, i);
                }
                else if (dist < 0)
                {
                    pow += 1 / IntPower(2, i);
                }
                else
                {
                    return pow;
                }

                i++;
            } while (Absolute(dist) > 0.00001);

            return pow;*/
        }

        public static int Sign(double x)
        {
            if (x > 0)
            {
                return 1;
            }

            if (x < 0)
            {
                return -1;
            }

            return 0;
        }

        public static double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        public static double Min(double a, double b)
        {
            return a < b ? a : b;
        }

        public static double AGM(double x, double y)
        {
            double a = (x + y) / 2;
            double g = SquareRoot(x * y);

            while (Absolute(a - g) > 0.001)
            {
                a = (a + g) / 2;
                g = SquareRoot(a * g);
            }

            return a;
        }

        public static double Round(double x)
        {
            return (int)(x + 0.5);
        }

        public static double Ceiling(double x)
        {
            return (int)(x + 0.9999999);
        }

        public static int Floor(double x)
        {
            return (int)x;
        }

        public static double Absolute(double x)
        {
            if (x < 0)
                x *= -1;
            return x;
        }

        public static bool IsPowerOfTwo(ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
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

        public static double SquareRoot(double a)
        {
            double xk = 1;

            for (int i = 0; i < 16; i++)
            {
                xk = (xk + a / xk) / 2;
            }

            return xk;
        }

        public static double NthRoot(double a, double n)
        {
            double xk = 1;

            int precision;

            if (n >= 1)
            {
                precision = (int)(n * n * 4); 
            }
            else
            {
                precision = (int)(4 / (n * n * n * n));
            }

            for (int i = 0; i < precision; i++)
            {
                xk = ((n - 1) * xk + (a / Power(xk, n - 1))) / n;
            }

            return xk;
        }

        public static double IntPower(double b, int n)
        {
            double sum = 1;

            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum *= b;
                }
            }
            else if (n < 0)
            {
                sum = 1 / IntPower(b, -n);
            }

            return sum;
        }

        public static Complex IntPower(Complex b, int n)
        {
            Complex sum = new Complex(1, 0);

            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    sum *= b;
                }
            }

            return sum;
        }

        public static int CountDigits(int x)
        {
            int count = 0;

            x = (int)Absolute(x);

            while (x > 0)
            {
                x /= 10;
                count++;
            }

            return count;
        }

        public static int CountDigits(double x)
        {
            return x.ToString().Length;
        }
    }
}
