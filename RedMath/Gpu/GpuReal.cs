using System;

using RedMath.Gpu;

namespace RedMath.Structures
{
    public partial class Real : IGpuCompatibleField<Real, GpuReal>
    {
        public IGpuStructManager<Real, GpuReal> GetDefaultGpuStructManager()
        {
            return new GpuRealManager();
        }
    }

    public struct GpuReal
    {
        public double Value;

        public GpuReal(double value)
        {
            Value = value;
        }
    }

    public class GpuRealManager : IGpuStructManager<Real, GpuReal>
    {
        public Real ToClass(GpuReal st)
        {
            return new Real(st.Value);
        }

        public GpuReal ToStruct(Real cl)
        {
            return new GpuReal(cl.Value);
        }

        public Func<GpuReal, GpuReal, GpuReal> GetStructAddition()
        {
            return (left, right) => new GpuReal { Value = left.Value + right.Value };
        }

        public Func<GpuReal, GpuReal, GpuReal> GetStructMultiplication()
        {
            return (left, right) => new GpuReal { Value = left.Value * right.Value };
        }

        public GpuReal GetStructDefaultValue()
        {
            return new GpuReal(0);
        }
    }
}
