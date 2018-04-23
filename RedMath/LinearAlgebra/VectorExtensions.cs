using RedMath.Structures;
using System;

namespace RedMath.LinearAlgebra
{
    public static class VectorExtensions
    {
        public static Vector<Real> GetNormalizedVector(this Vector<Real> vec)
        {
            return vec / vec.GetMagnitude();
        }

        public static Real GetMagnitude(this Vector<Real> vec)
        {
            return Math.Sqrt(Vector<Real>.DotProduct(vec, vec));
        }

        public static Vector<Complex> GetNormalizedVector(this Vector<Complex> vec)
        {
            return vec / vec.GetMagnitude();
        }

        public static Complex GetMagnitude(this Vector<Complex> vec)
        {
            return Vector<Complex>.DotProduct(vec, vec).SquareRoot;
        }
    }
}
