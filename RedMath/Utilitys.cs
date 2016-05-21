namespace RedMath
{
    public delegate double RealFunctionPlot(double[] args);

    public static class Utilitys
    {
        public static VectorSpace Plot(RealFunctionPlot plotter, int[] min, int[] max)
        {
            VectorSpace results;
            int resultsCount;
            int currentIndex = 0;

            int[] indices = new int[max.Length];
            int[] loopLimit = new int[indices.Length];

            for (int i = 0; i < indices.Length; i++) { indices[i] = 0; loopLimit[i] = max[i] - min[i]; }

            resultsCount = IndicesToInt(loopLimit, loopLimit);

            results = new VectorSpace(resultsCount);
            for (int i = 0; i < resultsCount; i++) { results[i] = new Vector(max.Length + 1); }

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
    }


    public struct VectorSpace
    {
        private Vector[] vertices;

        public VectorSpace(int count)
        {
            vertices = new Vector[count];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector();
            }
        }

        public Vector this[int vec]
        {
            get
            {
                return vertices[vec];
            }

            set
            {
                vertices[vec] = value;
            }
        }

        public double this[int vec, int comp]
        {
            get
            {
                return vertices[vec][comp];
            }

            set
            {
                vertices[vec][comp] = value;
            }
        }
    }

    public class Vector
    {
        double[] components;

        public int Dimension { get; private set; }

        public double Last
        {
            get
            {
                return components[components.Length - 1];
            }

            set
            {
                components[components.Length - 1] = value;
            }
        }

        public Vector(int dim)
        {
            Dimension = dim;
            components = new double[Dimension];

            for (int i = 0; i < dim; i++)
            {
                components[i] = 0;
            }
        }

        public Vector(params double[] comp)
        {
            Dimension = comp.Length;
            components = new double[Dimension];

            for (int i = 0; i < comp.Length; i++)
            {
                components[i] = comp[i];
            }
        }

        public double this[int index]
        {
            get
            {
                return components[index];
            }

            set
            {
                components[index] = value;
            }
        }
    }
}
