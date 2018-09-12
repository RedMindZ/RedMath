using System;
using RedMath.LinearAlgebra;

namespace RedMath.Structures
{
    public partial class Complex : Field<Complex>
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public Complex SquareRoot
        {
            get
            {
                double r = Math.Sqrt((Real + Math.Sqrt(Real * Real + Imaginary * Imaginary)) / 2);
                double i = Math.Sign(Imaginary) * Math.Sqrt((-Real + Math.Sqrt(Real * Real + Imaginary * Imaginary)) / 2);

                return new Complex(r, i);
            }
        }

        public Complex Reciprocal => new Complex(Real / (Real * Real + Imaginary * Imaginary), -Imaginary / (Real * Real + Imaginary * Imaginary));

        public double AbsoluteValue => Math.Sqrt(Real * Real + Imaginary * Imaginary);

        public double Phase => Math.Atan2(Real, Imaginary);

        protected override Complex _zero => new Complex(0, 0);

        protected override Complex _one => new Complex(1, 0);

        public override Complex AdditiveInverse => new Complex(-Real, -Imaginary);

        public override Complex MultiplicativeInverse => Reciprocal;

        public Complex()
        {
            Real = 0;
            Imaginary = 0;
        }

        public Complex(double real)
        {
            Real = real;
            Imaginary = 0;
        }

        public Complex(double real, double imaginary)
        {
            Real = Math.Round(real, 3);
            Imaginary = Math.Round(imaginary, 3);
        }

        public Complex(Complex other)
        {
            Real = other.Real;
            Imaginary = other.Imaginary;
        }

        public Complex Conjugate => new Complex(Real, -Imaginary);

        public static bool operator ==(Complex a, Complex b) => a.Equals(b);

        public static bool operator !=(Complex a, Complex b) => !(a.Equals(b));

        public static implicit operator Vector<Real>(Complex a) => new Vector<Real>(a.Real, a.Imaginary);

        public static implicit operator Complex(Real a) => new Complex(a, 0);

        public override bool Equals(object obj)
        {
            if (!(obj is Complex instance))
            {
                return false;
            }

            return Equals(instance);
        }

        public override bool Equals(Complex other)
        {
            if (Real == other.Real && Imaginary == other.Imaginary)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)(Real + Imaginary);
        }

        public override string ToString()
        {
            string rStr = "";
            string iStr = "";

            if (Real == (int)Real)
            {
                rStr = ((int)Real).ToString();
            }
            else
            {
                rStr = Real.ToString("0.###");
            }

            if (Imaginary == (int)Imaginary)
            {
                iStr = ((int)Imaginary).ToString() + "i";
            }
            else
            {
                iStr = Imaginary.ToString("0.###") + "i";
            }

            if (Imaginary == 1)
            {
                iStr = "i";
            }

            if (Imaginary == -1)
            {
                iStr = "-i";
            }

            if (Real == 0 && Imaginary == 0)
            {
                return "0";
            }

            if (Real == 0)
            {
                return iStr;
            }

            if (Imaginary == 0)
            {
                return rStr;
            }

            if (Imaginary > 0)
            {
                return rStr + "+" + iStr;
            }
            else
            {
                return rStr + iStr;
            }
        }

        public override Complex Add(Complex other)
        {
            return new Complex(Real + other.Real, Imaginary + other.Imaginary);
        }

        public override Complex Multiply(Complex other)
        {
            return new Complex(Real * other.Real - Imaginary * other.Imaginary, Imaginary * other.Real + Real * other.Imaginary);
        }

        public override Complex Clone()
        {
            return new Complex(this);
        }
    }
}
