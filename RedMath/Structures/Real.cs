using System;
using RedMath.ParallelComputation.GpuUtils;

namespace RedMath.Structures
{
    public class Real : Field<Real>, IGpuCompatibleField<Real, GpuReal>
    {
        public double Value { get; set; }

        public override Real Zero
        {
            get
            {
                return new Real(0);
            }
        }

        public override Real One
        {
            get
            {
                return new Real(1);
            }
        }

        public override Real AdditiveInverse
        {
            get
            {
                return -this;
            }
        }

        public override Real MultiplicativeInverse
        {
            get
            {
                if (this != 0)
                {
                    return 1 / this;
                }
                else
                {
                    throw new DivideByZeroException("Zero has no multiplicative inverse.");
                }
            }
        }

        public Real()
        {
            Value = 0;
        }

        public Real(double val)
        {
            Value = val;
        }

        public static bool operator ==(Real a, Real b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Real a, Real b)
        {
            return a.Value != b.Value;
        }

        public override Real Add(Real other)
        {
            return new Real(Value + other.Value);
        }

        public override Real Multiply(Real other)
        {
            return new Real(Value * other.Value);
        }

        public override bool Equals(object obj)
        {
            Real other = obj as Real;

            if ((object)other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override bool Equals(Real other)
        {
            if (Value == other.Value)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }

        public static implicit operator Real(double d)
        {
            return new Real(d);
        }

        public static implicit operator double(Real r)
        {
            return r.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override Real Clone()
        {
            return new Real(Value);
        }

        public IGpuStructManager<Real, GpuReal> GetDefaultGpuStructManager()
        {
            return new RealGpuTypeManager();
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

    public class RealGpuTypeManager : IGpuStructManager<Real, GpuReal>
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
