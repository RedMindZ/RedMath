using System;
using RedMath.Structures;
using RedMath.LinearAlgebra;

namespace RedMath.Numerics
{
    public class Complex : Field<Complex>
    {
        public double Real;
        public double Imaginary;

        public Complex SquareRoot
        {
            get
            {
                double r = Math.Sqrt((Real + Math.Sqrt(Real * Real + Imaginary * Imaginary)) / 2);
                double i = Math.Sign(Imaginary) * Math.Sqrt((-Real + Math.Sqrt(Real * Real + Imaginary * Imaginary)) / 2);

                return new Complex(r, i);
            }
        }

        public Complex Reciprocal
        {
            get
            {
                return new Complex(Real / (Real * Real + Imaginary * Imaginary), -Imaginary / (Real * Real + Imaginary * Imaginary));
            }
        }

        public double AbsoluteValue
        {
            get
            {
                return Math.Sqrt(Real * Real + Imaginary * Imaginary);
            }
        }

        public double Phase
        {
            get
            {
                return Math.Atan2(Real, Imaginary);
            }
        }

        public override Complex Zero
        {
            get
            {
                return new Complex(0, 0);
            }
        }

        public override Complex One
        {
            get
            {
                return new Complex(1, 0);
            }
        }

        public override Complex AdditiveInverse
        {
            get
            {
                return -this;
            }
        }

        public override Complex MultiplicativeInverse
        {
            get
            {
                return this.Reciprocal;
            }
        }

        public Complex()
        {
            Real = 0;
            Imaginary = 0;
        }

        public Complex(double RealImaginary)
        {
            Real = RealImaginary;
            Imaginary = RealImaginary;
        }

        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public void Conjugate()
        {
            Imaginary *= -1;
        }

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Imaginary * b.Real + a.Real * b.Imaginary);
        }

        public static Complex operator /(Complex a, Complex b)
        {
            return new Complex((a.Real * b.Real + a.Imaginary * b.Imaginary) / (b.Real * b.Real + b.Imaginary * b.Imaginary), (a.Imaginary * b.Real - a.Real * b.Imaginary) / (b.Real * b.Real + b.Imaginary * b.Imaginary));
        }

        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Real, -a.Imaginary);
        }

        public static bool operator ==(Complex a, Complex b)
        {
            if (((object)a == null) ^ ((object)b == null))
            {
                return false;
            }

            if (((object)a == null) && ((object)b == null))
            {
                return true;
            }

            if (a.Real == b.Real && a.Imaginary == b.Imaginary)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(Complex a, Complex b)
        {
            return !(a == b);
        }

        public static implicit operator Vector<Real>(Complex a)
        {
            return new Vector<Real>(a.Real, a.Imaginary);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Complex instance = (Complex)obj;

            if (this.Real == instance.Real && this.Imaginary == instance.Imaginary)
            {
                return true;
            }

            return false;
        }

        public override bool Equals(Complex other)
        {
            if (this.Real == other.Real && this.Imaginary == other.Imaginary)
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
            if (Imaginary >= 0)
            {
                return Real.ToString("0.000") + "+" + Imaginary.ToString("0.000") + "i"; 
            }
            else
            {
                return Real.ToString("0.000") + Imaginary.ToString("0.000") + "i";
            }
        }

        public override Complex Add(Complex element)
        {
            return this + element;
        }

        public override Complex Multiply(Complex element)
        {
            return this * element;
        }
    }
}
