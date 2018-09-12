using System;

namespace RedMath.Structures
{
    public partial class Rational : Field<Rational>
    {
        public long Numerator { get; private set; }

        private long _denominator = 1;
        public long Denominator
        {
            get
            {
                return _denominator;
            }

            private set
            {
                if (value != 0)
                {
                    _denominator = value;
                }
                else
                {
                    throw new ArgumentException("The value of a denominator must be different then zero.");
                }
            }
        }


        public Rational ReducedForm
        {
            get
            {
                Rational q = new Rational(Numerator, Denominator);
                q.Reduce();

                return q;
            }
        }

        public double Value => (double)Numerator / Denominator;

        protected override Rational _zero => new Rational(0);

        protected override Rational _one => new Rational(1);

        public override Rational AdditiveInverse => new Rational(-Numerator, Denominator);

        public override Rational MultiplicativeInverse => new Rational(Denominator, Numerator);

        public Rational()
        {
            Numerator = 0;
            Denominator = 1;
        }

        public Rational(long numerator, long denominator = 1)
        {
            Numerator = numerator;
            Denominator = denominator;

            Reduce();
        }

        public Rational(double number, int precision = 8)
        {
            Denominator = Algebra.IntPower(10, precision);
            Numerator = (long)(number * Denominator);

            Reduce();
        }

        public void Reduce()
        {
            long GDC = Algebra.GreatestCommonDivisor(Numerator, Denominator);

            Numerator /= GDC;
            Denominator /= GDC;

            if (Denominator < 0)
            {
                Denominator *= -1;
                Numerator *= -1;
            }
        }

        public override Rational Add(Rational other)
        {
            return new Rational(Numerator * other.Denominator + Denominator * other.Numerator, Denominator * other.Denominator).ReducedForm;
        }

        public override Rational Multiply(Rational other)
        {
            return new Rational(Numerator * other.Numerator, Denominator * other.Denominator).ReducedForm;
        }

        public override Rational Clone()
        {
            return new Rational(Numerator, Denominator);
        }

        public static implicit operator Rational(int num)
        {
            return new Rational(num);
        }

        public static implicit operator Rational(long num)
        {
            return new Rational(num);
        }

        public static implicit operator double(Rational q)
        {
            return q.Value;
        }

        public override bool Equals(Rational other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator;
        }

        public override string ToString()
        {
            string str = "";

            str += Numerator;

            if (Denominator != 1)
            {
                str += "/" + Denominator;
            }

            return str;
        }
    }

}