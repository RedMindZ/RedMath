using System;

using RedMath.Gpu;

namespace RedMath.Structures
{
    public partial class Rational : IGpuCompatibleField<Rational, GpuRational>
    {
        public IGpuStructManager<Rational, GpuRational> GetDefaultGpuStructManager()
        {
            return new GpuRationalManager();
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

    public class GpuRationalManager : IGpuStructManager<Rational, GpuRational>
    {
        public GpuRational ToStruct(Rational cl)
        {
            return new GpuRational(cl.Numerator, cl.Denominator);
        }

        public Rational ToClass(GpuRational st)
        {
            return new Rational(st.Numerator, st.Denominator);
        }

        public Func<GpuRational, GpuRational, GpuRational> GetStructAddition()
        {
            return (left, right) => new GpuRational
            {
                Numerator = left.Numerator * right.Denominator + left.Denominator * right.Numerator,
                Denominator = left.Denominator * right.Denominator
            };
        }

        public Func<GpuRational, GpuRational, GpuRational> GetStructMultiplication()
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
