using System;

namespace RedMath
{
    public static class Trigonometry
    {
        public static double Sin(double x, bool round = false)
        {
            if (!round)
            {
                return Math.Sin(x);
            }
            else
            {
                double res = Math.Sin(x);

                if (Algebra.Absolute(res) < 0.00001)
                {
                    return 0;
                }

                return res;
            }
        }

        public static double Cos(double x, bool round = false)
        {
            if (!round)
            {
                return Math.Cos(x); 
            }
            else
            {
                double res = Math.Cos(x);

                if (Algebra.Absolute(res) < 0.00001)
                {
                    return 0;
                }

                return res;
            }
        }

        public static double Arctan2(double x, double y)
        {
            return Math.Atan2(y, x);
        }
    }
}
