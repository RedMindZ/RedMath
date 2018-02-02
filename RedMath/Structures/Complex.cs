﻿using System;
using RedMath.Structures;
using RedMath.LinearAlgebra;

namespace RedMath.Structures
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

        public Complex(double scalar)
        {
            Real = scalar;
            Imaginary = scalar;
        }

        public Complex(double real, double imaginary)
        {
            Real = Math.Round(real, 3);
            Imaginary = Math.Round(imaginary, 3);
        }

        public Complex Conjugate
        {
            get
            {
                return new Complex(Real, -Imaginary);
            }
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
            return a.Equals(b);
        }

        public static bool operator !=(Complex a, Complex b)
        {
            return !(a.Equals(b));
        }

        public static implicit operator Vector<Real>(Complex a)
        {
            return new Vector<Real>(a.Real, a.Imaginary);
        }

        public static implicit operator Complex(Real a)
        {
            return new Complex(a, 0);
        }

        public override bool Equals(object obj)
        {
            Complex instance = obj as Complex;

            if (instance == null)
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