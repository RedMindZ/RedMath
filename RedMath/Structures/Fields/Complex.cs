using System;
using RedMath.LinearAlgebra;

namespace RedMath.Structures
{
    public partial class Complex : Field<Complex>
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        protected override Complex _zero => new Complex(0, 0);
        protected override Complex _one => new Complex(1, 0);

        public override Complex AdditiveInverse => new Complex(-Real, -Imaginary);
        public override Complex MultiplicativeInverse => new Complex(Real / (Real * Real + Imaginary * Imaginary), -Imaginary / (Real * Real + Imaginary * Imaginary));

        public double AbsoluteValue => Math.Sqrt(Real * Real + Imaginary * Imaginary);
        public double Phase => Math.Atan2(Real, Imaginary);

        public Complex Conjugate => new Complex(Real, -Imaginary);


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

        public override Complex Add(Complex other) => new Complex(Real + other.Real, Imaginary + other.Imaginary);
        public override Complex Multiply(Complex other) => new Complex(Real * other.Real - Imaginary * other.Imaginary, Imaginary * other.Real + Real * other.Imaginary);

        public Complex[] GetRoots(int degree)
        {
            Complex[] roots = new Complex[degree];

            double abs = Math.Pow(AbsoluteValue, 1D/degree);
            double phase = Phase;

            for (int i = 0; i < degree; i++)
            {
                double p = (phase + 2 * Math.PI * i) / degree;
                roots[i] = new Complex(abs * Math.Cos(p), abs * Math.Sin(p));
            }

            return roots;
        }

        public override bool Equals(Complex other) => Real == other.Real && Imaginary == other.Imaginary;
        public override Complex Clone() => new Complex(Real, Imaginary);

        public static implicit operator Vector<Real>(Complex a) => new Vector<Real>(a.Real, a.Imaginary);
        public static implicit operator Complex(Real a) => new Complex(a, 0);

        public override int GetHashCode() => (int)(Real + Imaginary);

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
    }
}
