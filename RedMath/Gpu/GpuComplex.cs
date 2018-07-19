using System;

using RedMath.Gpu;

namespace RedMath.Structures
{
    public partial class Complex : IGpuCompatibleField<Complex, GpuComplex>
    {
        public IGpuStructManager<Complex, GpuComplex> GetDefaultGpuStructManager()
        {
            return new GpuComplexManager();
        }
    }

    public struct GpuComplex
    {
        public double Real;
        public double Imaginary;
    }

    public class GpuComplexManager : IGpuStructManager<Complex, GpuComplex>
    {
        public Func<GpuComplex, GpuComplex, GpuComplex> GetStructAddition()
        {
            return (left, right) => new GpuComplex
            {
                Real = left.Real + right.Real,
                Imaginary = left.Imaginary + right.Imaginary
            };
        }

        public Func<GpuComplex, GpuComplex, GpuComplex> GetStructMultiplication()
        {
            return (left, right) => new GpuComplex
            {
                Real = left.Real * right.Real - left.Imaginary * right.Imaginary,
                Imaginary = left.Imaginary * right.Real + left.Real * right.Imaginary
            };
        }

        public Complex ToClass(GpuComplex st)
        {
            return new Complex(st.Real, st.Imaginary);
        }

        public GpuComplex ToStruct(Complex cl)
        {
            return new GpuComplex { Real = cl.Real, Imaginary = cl.Imaginary };
        }

        public GpuComplex GetStructDefaultValue()
        {
            return new GpuComplex();
        }
    }
}
