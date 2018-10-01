using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedMath.Utils
{
    public static class Utilities
    {


        public static T[] Slice<T>(this T[] source, int fromInclusive, int toExclusive)
        {
            if (toExclusive < 0)
            {
                toExclusive += source.Length;
            }

            int len = toExclusive - fromInclusive;

            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + fromInclusive];
            }
            return res;
        }

        public static T Sum<T>(this IEnumerable<T> sequence) where T : IAddable<T>
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();

            if (enumerator.MoveNext())
            {
                T sum = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    sum = sum.Add(enumerator.Current);
                }

                return sum;
            }
            else
            {
                throw new ArgumentException("Can't take the sum of an empty sequence.");
            }
        }

        public static T Product<T>(this IEnumerable<T> sequence) where T : IMultiplicable<T>
        {
            IEnumerator<T> enumerator = sequence.GetEnumerator();

            if (enumerator.MoveNext())
            {
                T sum = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    sum = sum.Multiply(enumerator.Current);
                }

                return sum;
            }
            else
            {
                throw new ArgumentException("Can't take the product of an empty sequence.");
            }
        }

        public static void AssignAll<T>(this Array arr, T value)
        {
            arr.AssignAll(indArr => value);
        }

        public static void AssignAll<T>(this Array arr, Func<int[], T> initFunc)
        {
            int[] minIndeces = new int[arr.Rank];
            int[] maxIndeces = new int[arr.Rank];

            for (int i = 0; i < arr.Rank; i++)
            {
                minIndeces[i] = arr.GetLowerBound(i);
                maxIndeces[i] = arr.GetUpperBound(i) + 1;
            }

            arr.Assign(initFunc, minIndeces, maxIndeces);
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, int toExclusive)
        {
            arr.Assign(initFunc, new MultiIndex(toExclusive, arr.Rank));
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, int fromInclusive, int toExclusive)
        {
            arr.Assign(initFunc, new MultiIndex(fromInclusive, toExclusive, arr.Rank));
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, int fromInclusive, int toExclusive, int steps)
        {
            arr.Assign(initFunc, new MultiIndex(fromInclusive, toExclusive, steps, arr.Rank));
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, int[] toExclusiveArr)
        {
            arr.Assign(initFunc, new MultiIndex(toExclusiveArr));
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, int[] fromInclusiveArr, int[] toExclusiveArr)
        {
            arr.Assign(initFunc, new MultiIndex(fromInclusiveArr, toExclusiveArr));
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, int[] fromInclusiveArr, int[] toExclusiveArr, int[] stepsArr)
        {
            arr.Assign(initFunc, new MultiIndex(fromInclusiveArr, toExclusiveArr, stepsArr));
        }

        public static void Assign<T>(this Array arr, Func<int[], T> initFunc, MultiIndex mindex)
        {
            if (arr.GetType().GetElementType() != typeof(T))
            {
                throw new ArgumentException("The type of the array must be the same as the type that " + nameof(initFunc) + " returns.");
            }

            while (mindex.Increment())
            {
                int[] indices = mindex.Indices.ToArray();
                arr.SetValue(initFunc(indices), indices);
            }
        }
    }


}
