using RedMath.Structures;
using System;

namespace RedMath.LinearAlgebra
{
    public class Vector<T> where T : Field<T>, new()
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

                    components = temp;
                }
            }
        }

        public T X
        {
            get
            {
                return components[0];
            }
        }

        public T Y
        {
            get
            {
                return components[1];
            }
        }

        public T Z
        {
            get
            {
                return components[2];
            }
        }

        public T W
        {
            get
            {
                return components[3];
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

        public double Magnitude
        {
            get
            {
                if (this is Vector<Real>)
                {
                    double sum = 0;

                    for (int i = 0; i < Dimension; i++)
                    {
                        sum += (this as Vector<Real>)[i].Multiply((this as Vector<Real>)[i]);
                    }

                    return Math.Sqrt(sum);
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public Vector<T> Opposite
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
                components[i] = new T();
            }
        }

        public Vector(params T[] comp)
        {
            components = new T[comp.Length];

            for (int i = 0; i < comp.Length; i++)
            {
                components[i] = comp[i];
            }
        }
        
        public T Sum()
        {
            T sum = new T();

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
                temp[0, i] = this[i];
            }

            return new Matrix<T>(temp);
        }

        public Matrix<T> ToColumnMatrix()
        {
            T[,] temp = new T[Dimension, 1];

            for (int i = 0; i < Dimension; i++)
            {
                temp[i, 0] = this[i];
            }

            return new Matrix<T>(temp);
        }

        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
        {
            Vector<T> vec = new Vector<T>((int)Math.Max(a.Dimension, b.Dimension));

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

            return vec;
        }

        public static Vector<T> operator -(Vector<T> a)
        {
            return a.Opposite;
        }

        public static Vector<T> operator -(Vector<T> a, Vector<T> b)
        {
            Vector<T> vec = new Vector<T>((int)Math.Max(a.Dimension, b.Dimension));

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

            return vec;
        }

        public static Vector<T> operator *(Vector<T> a, T d)
        {
            Vector<T> vec = new Vector<T>(a.components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] = vec[i].Multiply(d);
            }

            return vec;
        }

        public static Vector<T> operator *(T d, Vector<T> a)
        {
            Vector<T> vec = new Vector<T>(a.components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] = vec[i].Multiply(d);
            }

            return vec;
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

        public static Vector<T> operator /(T d, Vector<T> a)
        {
            Vector<T> vec = new Vector<T>(a.components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] = vec[i].Divide(d);
            }

            return vec;
        }

        public static T DotProduct(Vector<T> a, Vector<T> b)
        {
            T sum = new T().Zero;

            for (int i = 0; i < Math.Min(a.Dimension, b.Dimension); i++)
            {
                sum.Add(a[i].Multiply(b[i]));
            }

            return sum;
        }

        public T DotProduct(Vector<T> a)
        {
            return DotProduct(this, a);
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
                temp[i] = vec[i];
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

            return (int)Magnitude;
        }
    }
}
