using RedMath.Structures;
using System;

namespace RedMath.LinearAlgebra
{
    /// <summary>
    /// Potentialy usefull class for implementing special vector spaces like R[x] (the polynomial vector space).
    /// Currently in development and may not be ready for use.
    /// </summary>
    public abstract class VectorBase<T, F> where T : VectorBase<T, F> where F : Field<F>
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

    public class Vector<T> : VectorBase<Vector<T>, T> where T : Field<T>, new()
    {
        private T[] components;

        public int Dimension
        {
            get
            {
                return components.Length;
            }

            set
            {
                if (value < Dimension)
                {
                    T[] temp = new T[value];

                    for (int i = 0; i < value; i++)
                    {
                        temp[i] = components[i];
                    }

                    components = temp;
                }

                if (value > Dimension)
                {
                    T[] temp = new T[value];

                    for (int i = 0; i < Dimension; i++)
                    {
                        temp[i] = components[i];
                    }

                    for (int i = Dimension; i < temp.Length; i++)
                    {
                        temp[i] = new T().Zero.Clone();
                    }

                    components = temp;
                }
            }
        }

        public override Vector<T> Zero
        {
            get
            {
                return new Vector<T>(Dimension);
            }
        }

        public T X
        {
            get
            {
                return components[0];
            }

            set
            {
                components[0] = value;
            }
        }

        public T Y
        {
            get
            {
                return components[1];
            }

            set
            {
                components[1] = value;
            }
        }

        public T Z
        {
            get
            {
                return components[2];
            }

            set
            {
                components[2] = value;
            }
        }

        public T Last
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

        public override Vector<T> AddativeInverse
        {
            get
            {
                Vector<T> vec = new Vector<T>(components);

                for (int i = 0; i < vec.Dimension; i++)
                {
                    vec[i] = vec[i].AdditiveInverse;
                }

                return vec;
            }
        }

        public Vector<T> HomogeneousCoordinates
        {
            get
            {
                T[] comp = new T[Dimension + 1];

                for (int i = 0; i < components.Length; i++)
                {
                    comp[i] = components[i];
                }

                comp[comp.Length - 1] = new T().One;

                return new Vector<T>(comp);
            }
        }

        public T this[int index]
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

        public Vector(int dim)
        {
            components = new T[dim];

            for (int i = 0; i < dim; i++)
            {
                components[i] = new T().Zero.Clone();
            }
        }

        public Vector(params T[] comp)
        {
            components = new T[comp.Length];

            for (int i = 0; i < comp.Length; i++)
            {
                components[i] = comp[i].Clone();
            }
        }

        public Vector(Vector<T> other) : this((T[])other) { }

        public T Sum()
        {
            T sum = new T().Zero;

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

            if (vec.Last == new T().Zero)
            {
                for (int i = 0; i < normalVec.Dimension; i++)
                {
                    normalVec[i] = vec[i];
                }

                return normalVec;
            }

            for (int i = 0; i < normalVec.Dimension; i++)
            {
                normalVec[i] = vec[i].Divide(vec.Last);
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
                vectorSum[i] = vectorSum[i].Add(components[i]);
                vectorSum[i] = vectorSum[i].Add(other[i]);
            }

            return vectorSum;
        }

        public override Vector<T> MultiplyByScalar(T scalar)
        {
            Vector<T> scaledVector = new Vector<T>(components);

            for (int i = 0; i < scaledVector.Dimension; i++)
            {
                scaledVector[i] = scaledVector[i].Multiply(scalar);
            }

            return scaledVector;
        }

        public static Vector<T> operator +(Vector<T> left, Vector<T> right)
        {
            /*Vector<T> vec = new Vector<T>(Math.Max(a.Dimension, b.Dimension));

            for (int i = 0; i < vec.Dimension; i++)
            {
                if (i < a.Dimension)
                {
                    vec[i] = vec[i].Add(a[i]);
                }

                if (i < b.Dimension)
                {
                    vec[i] = vec[i].Add(b[i]);
                }
            }

            return vec;*/

            return left.Add(right);
        }

        public static Vector<T> operator -(Vector<T> vec)
        {
            return vec.AddativeInverse;
        }

        public static Vector<T> operator -(Vector<T> left, Vector<T> right)
        {
            /*Vector<T> vec = new Vector<T>((int)Math.Max(a.Dimension, b.Dimension));

            for (int i = 0; i < vec.Dimension; i++)
            {
                if (i < a.Dimension)
                {
                    vec[i] = vec[i].Add(a[i]);
                }

                if (i < b.Dimension)
                {
                    vec[i] = vec[i].Subtract(b[i]);
                }
            }

            return vec;*/

            return left.Subtract(right);
        }

        public static Vector<T> operator *(Vector<T> vec, T scalar)
        {
            /*Vector<T> scaledVector = new Vector<T>(vec.components);

            for (int i = 0; i < scaledVector.Dimension; i++)
            {
                scaledVector[i] = scaledVector[i].Multiply(scalar);
            }

            return scaledVector;*/

            return vec.MultiplyByScalar(scalar);
        }

        public static Vector<T> operator *(T scalar, Vector<T> vec)
        {
            /*Vector<T> scaledVector = new Vector<T>(vec.components);

            for (int i = 0; i < scaledVector.Dimension; i++)
            {
                scaledVector[i] = scaledVector[i].Multiply(scalar);
            }

            return scaledVector;*/

            return vec.MultiplyByScalar(scalar);
        }

        public static Vector<T> operator /(Vector<T> a, T d)
        {
            Vector<T> vec = new Vector<T>(a.components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] = vec[i].Divide(d);
            }

            return vec;
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

        public static bool operator ==(Vector<T> a, Vector<T> b)
        {
            if (a.Dimension != b.Dimension)
            {
                return false;
            }

            for (int i = 0; i < a.Dimension; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Vector<T> a, Vector<T> b)
        {
            return !(a == b);
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

        public override string ToString()
        {
            string res = "(";

            for (int i = 0; i < Dimension; i++)
            {
                res += components[i];

                if (i < Dimension - 1)
                {
                    res += ", ";
                }
            }

            res += ")";

            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Vector<T> vec = obj as Vector<T>;

            if (vec.Dimension != this.Dimension)
            {
                return false;
            }

            for (int i = 0; i < vec.Dimension; i++)
            {
                if (vec[i] != this[i])
                {
                    return false;
                }
            }

            return true;
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