using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMath
{
    public delegate double RealFunctionPlot(double[] args);

    public static class Utilitys
    {
        public static Vector<Real>[] Plot(RealFunctionPlot plotter, int[] min, int[] max)
        {
            Vector<Real>[] results;
            int resultsCount;
            int currentIndex = 0;

            int[] indices = new int[max.Length];
            int[] loopLimit = new int[indices.Length];

            for (int i = 0; i < indices.Length; i++) { indices[i] = 0; loopLimit[i] = max[i] - min[i]; }

            resultsCount = IndicesToInt(loopLimit, loopLimit);

            results = new Vector<Real>[resultsCount];
            for (int i = 0; i < resultsCount; i++) { results[i] = new Vector<Real>(max.Length + 1); }

            double[] values = new double[indices.Length];

            for (; !IndiciesEnd(indices, loopLimit); IndiciesAdvance(indices, loopLimit))
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    values[i] = indices[i] + min[i];
                }

                for (int i = 0; i < results[currentIndex].Dimension - 1; i++)
                {
                    results[currentIndex][i] = values[i];
                }

                results[currentIndex].Last = plotter(values);

                currentIndex++;
            }

            return results;
        }

        private static int IndicesToInt(int[] indices, int[] limits)
        {
            int sum = indices[indices.Length - 1];
            int mul = 1;

            for (int i = indices.Length - 2; i >= 0; i--)
            {
                mul *= limits[i + 1];
                sum += indices[i] * mul;
            }

            return sum;
        }

        private static int[] IndiciesAdvance(int[] indicies, int[] maxValues)
        {
            int LastIndex = indicies.Length - 1;
            int LastMax = maxValues.Length - 1;

            if (indicies[LastIndex] < maxValues[LastMax])
            {
                indicies[LastIndex]++;
                return indicies;
            }
            else
            {
                if (LastIndex == 0)
                {
                    indicies[LastIndex] = 0;
                    return indicies;
                }
                else
                {
                    indicies[LastIndex] = 0;
                    int[] ind = IndiciesAdvance(indicies.Slice(0, LastIndex), maxValues.Slice(0, LastMax));
                    for (int i = 0; i < ind.Length; i++)
                    {
                        indicies[i] = ind[i];
                    }
                    return indicies;
                }
            }

        }

        private static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start;

            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }

        private static bool IndiciesEnd(int[] indicies, int[] maxValues)
        {
            for (int i = 0; i < indicies.Length; i++)
            {
                if (indicies[i] < maxValues[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static double SequenceSum(double[] seq)
        {
            double sum = 0;

            for (int i = 0; i < seq.Length; i++)
            {
                sum += seq[i];
            }

            return sum;
        }

        public static T SequenceProduct<T>(T[] seq) where T : Field<T>, new()
        {
            T prod = new T().One;

            for (int i = 0; i < seq.Length; i++)
            {
                prod = prod.Multiply(seq[i]);
            }

            return prod;
        }

        public static int CountDigits(double x)
        {
            return x.ToString().Length;
        }

        public static int CountDigits<T>(Field<T> x) where T : new()
        {
            return x.ToString().Length;
        }
    }

    
}
