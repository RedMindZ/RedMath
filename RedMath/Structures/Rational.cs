using System;

using RedMath.HighPerformance.Gpu;

namespace RedMath.Structures
{
    public class Rational : Field<Rational>, IGpuCompatibleField<Rational, GpuRational>
    {
        public long Numerator { get; set; }

        private long denominator = 1;
        public long Denominator
        {
            get
            {
                return denominator;
            }

            set
            {
                if (value != 0)
                {
                    denominator = value;
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

        public override Rational Zero => new Rational(0);

        public override Rational One => new Rational(1);

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

        public IGpuStructManager<Rational, GpuRational> GetDefaultGpuStructManager()
        {
            return new RationalGpuTypeManager();
        }
    }

    public struct GpuRational
    {
        public long Numerator;
        public long Denominator;

        public GpuRational(long numerator, long denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }

    public class RationalGpuTypeManager : IGpuStructManager<Rational, GpuRational>
    {
        public GpuRational ToStruct(Rational cl)
        {
            return new GpuRational(cl.Numerator, cl.Denominator);
        }

        public Rational ToClass(GpuRational st)
        {
            return new Rational(st.Numerator, st.Denominator);
        }

        Func<GpuRational, GpuRational, GpuRational> IFieldStruct<GpuRational>.GetStructAddition()
        {
            return (left, right) => new GpuRational
            {
                Numerator = left.Numerator * right.Denominator + left.Denominator * right.Numerator,
                Denominator = left.Denominator * right.Denominator
            };
        }

        Func<GpuRational, GpuRational, GpuRational> IFieldStruct<GpuRational>.GetStructMultiplication()
        {
            return (left, right) => new GpuRational
            {
                Numerator = left.Numerator * right.Numerator,
                Denominator = left.Denominator * right.Denominator
            };
        }

        public GpuRational GetStructDefaultValue()
        {
            return new GpuRational(0, 1);
        }
    }
}