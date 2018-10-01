using RedMath.Structures;
using System;

namespace RedMath.LinearAlgebra
{
    /// <summary>
    /// Potentialy usefull class for implementing special vector spaces like R[x] (the polynomial vector space).
    /// Currently in development and may not be ready for use.
    /// </summary>
    public abstract class VectorBase<T, F> where T : VectorBase<T, F> where F : Field<F>, new()
    {
        public abstract T Zero { get; }

        public abstract T AddativeInverse { get; }

        public abstract T Add(T other);
        public abstract T MultiplyByScalar(F scalar);

        public T Subtract(T other)
        {
            return Add(other.AddativeInverse);
        }
    }

    public class Vector<T> : DynamicDimensionCoordinateSpace<Vector<T>, T> where T : Field<T>, new()
    {
        public Vector<T> HomogeneousCoordinates
        {
            get
            {
                T[] comp = new T[Dimension + 1];

                for (int i = 0; i < Elements.Length; i++)
                {
                    comp[i] = Elements[i];
                }

                comp[comp.Length - 1] = Field<T>.One;

                return new Vector<T>(comp);
            }
        }

        public Vector() : base() { }
        public Vector(int dimension) : base(dimension) { }
        public Vector(params T[] elements) : base(elements) { }
        public Vector(Vector<T> other) : base(other.Elements) { }

        public T Sum()
        {
            T sum = Field<T>.Zero;

            for (int i = 0; i < Dimension; i++)
            {
                sum = sum.Add(this[i]);
            }

            return sum;
        }

        public Matrix<T> ToRowMatrix()
        {
            T[,] temp = new T[1, Dimension];

            for (int i = 0; i < Dimension; i++)
            {
                temp[0, i] = this[i].Clone();
            }

            return new Matrix<T>(temp);
        }

        public Matrix<T> ToColumnMatrix()
        {
            T[,] temp = new T[Dimension, 1];

            for (int i = 0; i < Dimension; i++)
            {
                temp[i, 0] = this[i].Clone();
            }

            return new Matrix<T>(temp);
        }

        public static Vector<T> HomogeneousCoordinatesToEuclidean(Vector<T> vec)
        {
            Vector<T> normalVec = new Vector<T>(vec.Dimension - 1);

            if (vec[vec.Elements.Length - 1] == Field<T>.Zero)
            {
                for (int i = 0; i < normalVec.Dimension; i++)
                {
                    normalVec[i] = vec[i];
                }

                return normalVec;
            }

            for (int i = 0; i < normalVec.Dimension; i++)
            {
                normalVec[i] = vec[i].Divide(vec[vec.Elements.Length - 1]);
            }

            return normalVec;
        }

        public override Vector<T> Add(Vector<T> other)
        {
            if (other.Dimension != Dimension)
            {
                throw new InvalidOperationException("Only two vectors of the same dimension can be added together.");
            }

            Vector<T> vectorSum = new Vector<T>(Dimension);

            for (int i = 0; i < vectorSum.Dimension; i++)
            {
                vectorSum[i] = vectorSum[i].Add(Elements[i]);
                vectorSum[i] = vectorSum[i].Add(other[i]);
            }

            return vectorSum;
        }

        public static Complex DotProduct(Vector<Complex> a, Vector<Complex> b)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("The dot product can only be applied to two vectors of the same dimension.");
            }

            Complex sum = new Complex();

            for (int i = 0; i < a.Dimension; i++)
            {
                sum = sum.Add(a[i].Multiply(b[i].Conjugate));
            }

            return sum;
        }

        public static Real DotProduct(Vector<Real> a, Vector<Real> b)
        {
            if (a.Dimension != b.Dimension)
            {
                throw new ArgumentException("The dot product can only be applied to two vectors of the same dimension.");
            }

            Real sum = 0;

            for (int i = 0; i < a.Dimension; i++)
            {
                sum = sum.Add(a[i].Multiply(b[i]));
            }

            return sum;
        }

        public static Vector<T> CrossProduct(params Vector<T>[] vectors)
        {
            if (vectors.Length < 2)
            {
                throw new ArgumentException("Cross product requires at least 2 vectors", "vectors");
            }

            for (int i = 1; i < vectors.Length; i++)
            {
                if (vectors[i - 1].Dimension != vectors[i].Dimension)
                {
                    throw new ArgumentException("Cross product requires that all the vectors will be of the same dimesion");
                }
            }

            if (vectors[0].Dimension != vectors.Length + 1)
            {
                throw new ArgumentException("Cross product requires the count of the vectors to be one less then their dimension");
            }

            Matrix<T> mat = new Matrix<T>();

            foreach (Vector<T> vec in vectors)
            {
                mat.AppendColumnVector(vec);
            }

            Vector<T> sum = new Vector<T>(vectors[0].Dimension);

            for (int i = 0; i < sum.Dimension; i++)
            {
                sum[i] = sum[i].Add(mat.Minor(i, -1));

                if (i % 2 == 1)
                {
                    sum[i] = sum[i].AdditiveInverse;
                }
            }

            return sum;
        }

        public static implicit operator T[] (Vector<T> vec)
        {
            T[] temp = new T[vec.Dimension];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = vec[i].Clone();
            }

            return temp;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < Dimension; i++)
            {
                hash += this[i].GetHashCode();
            }

            return hash;
        }
    }
}