using RedMath.LinearAlgebra;
using RedMath.Structures;
using System;

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

        public static T FieldSum<T>(this T[] seq) where T : Field<T>, new()
        {
            T sum = new T().Zero;

            for (int i = 0; i < seq.Length; i++)
            {
                sum += seq[i];
            }

            return sum;
        }

        public static T FieldProduct<T>(this T[] seq) where T : Field<T>, new()
        {
            T prod = new T().One;

            for (int i = 0; i < seq.Length; i++)
            {
                prod = prod.Multiply(seq[i]);
            }

            return prod;
        }

        public static void Initialize<T>(this Array arr, T value)
        {
            arr.Initialize((indArr) => value);
        }

        public static void Initialize<T>(this Array arr, Func<int[], T> initFunc)
        {
            if (arr.GetType().GetElementType() != typeof(T))
            {
                throw new ArgumentException("The type of the array must be the same as the type that " + nameof(initFunc) + " returns.");
            }

            int[] minIndeces = new int[arr.Rank];
            int[] maxIndeces = new int[arr.Rank];

            for (int i = 0; i < arr.Rank; i++)
            {
                minIndeces[i] = arr.GetLowerBound(i);
                maxIndeces[i] = arr.GetUpperBound(i) + 1;
            }

            MultiIndex mindex = new MultiIndex(minIndeces, maxIndeces);

            foreach(int[] indices in mindex)
            {
                arr.SetValue(initFunc(indices), indices);
            }
        }
    }


}
