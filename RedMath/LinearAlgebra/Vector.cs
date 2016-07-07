namespace RedMath.LinearAlgebra
{
    public class Vector
    {
        double[] Components
        {
            get; set;
        }

        public int Dimension
        {
            get
            {
                return Components.Length;
            }
        }

        public double X
        {
            get
            {
                return Components[0];
            }
        }

        public double Y
        {
            get
            {
                return Components[1];
            }
        }

        public double Z
        {
            get
            {
                return Components[2];
            }
        }

        public double W
        {
            get
            {
                return Components[3];
            }
        }

        public double Last
        {
            get
            {
                return Components[Components.Length - 1];
            }

            set
            {
                Components[Components.Length - 1] = value;
            }
        }

        public double Magnitude
        {
            get
            {
                double sum = 0;

                for (int i = 0; i < Dimension; i++)
                {
                    sum += Algebra.IntPower(this[i], 2);
                }

                return Algebra.SquareRoot(sum);
            }
        }

        public Vector Opposite
        {
            get
            {
                Vector vec = new Vector(Components);

                for (int i = 0; i < vec.Dimension; i++)
                {
                    vec[i] = -vec[i];
                }

                return vec;
            }
        }

        public Vector UnitVector
        {
            get
            {
                return this / Magnitude;
            }
        }

        public double this[int index]
        {
            get
            {
                return Components[index];
            }

            set
            {
                Components[index] = value;
            }
        }

        public Vector(int dim)
        {
            Components = new double[Dimension];

            for (int i = 0; i < dim; i++)
            {
                Components[i] = 0;
            }
        }

        public Vector(params double[] comp)
        {
            Components = new double[Dimension];

            for (int i = 0; i < comp.Length; i++)
            {
                Components[i] = comp[i];
            }
        }

        public bool IsParallel(Vector a)
        {
            return a.UnitVector == this.UnitVector;
        }

        public bool IsAntiParallel(Vector a)
        {
            return a.UnitVector == -this.UnitVector;
        }

        public double Sum()
        {
            double sum = 0;

            for (int i = 0; i < Dimension; i++)
            {
                sum += this[i];
            }

            return sum;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            Vector vec = new Vector(Algebra.Max(a.Dimension, b.Dimension));

            for (int i = 0; i < vec.Dimension; i++)
            {
                if (i < a.Dimension)
                {
                    vec[i] += a[i];
                }

                if (i < b.Dimension)
                {
                    vec[i] += b[i];
                }
            }

            return vec;
        }

        public static Vector operator -(Vector a)
        {
            return a.Opposite;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            Vector vec = new Vector(Algebra.Max(a.Dimension, b.Dimension));

            for (int i = 0; i < vec.Dimension; i++)
            {
                if (i < a.Dimension)
                {
                    vec[i] += a[i];
                }

                if (i < b.Dimension)
                {
                    vec[i] -= b[i];
                }
            }

            return vec;
        }

        public static Vector operator *(Vector a, double d)
        {
            Vector vec = new Vector(a.Components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] *= d;
            }

            return vec;
        }

        public static Vector operator *(double d, Vector a)
        {
            Vector vec = new Vector(a.Components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] *= d;
            }

            return vec;
        }

        public static Vector operator /(Vector a, double d)
        {
            Vector vec = new Vector(a.Components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] /= d;
            }

            return vec;
        }

        public static Vector operator /(double d, Vector a)
        {
            Vector vec = new Vector(a.Components);

            for (int i = 0; i < vec.Dimension; i++)
            {
                vec[i] /= d;
            }

            return vec;
        }

        public static double DotProduct(Vector a, Vector b)
        {
            double sum = 0;

            for (int i = 0; i < Algebra.Min(a.Dimension, b.Dimension); i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        public double DotProduct(Vector a)
        {
            return DotProduct(this, a);
        }

        public static bool operator ==(Vector a, Vector b)
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

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            string res = "";

            for (int i = 0; i < Dimension; i++)
            {
                if (Components[i] >= 0 && i > 0)
                {
                    res += "+";
                }

                res += Components[i] + "e(" + i + ")";
            }

            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Vector vec = obj as Vector;

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
                hash += (int)this[i];
            }

            return hash;
        }
    }
}
