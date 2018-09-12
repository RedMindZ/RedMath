using System;

namespace RedMath.Structures
{
    public partial class Real : Field<Real>
    {
        public double Value { get; set; }

        protected override Real _zero => new Real(0);
        protected override Real _one => new Real(1);

        public override Real AdditiveInverse => new Real(-Value);

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

        public override Real Add(Real other) =>new Real(Value + other.Value);
        public override Real Multiply(Real other) => new Real(Value * other.Value);

        public override bool Equals(Real other)
        {
            if (Value == other.Value)
            {
                return true;
            }

            return false;
        }

        public override Real Clone() => new Real(Value);
        public override int GetHashCode() => (int)Value;

        public static implicit operator Real(double d) => new Real(d);
        public static implicit operator double(Real r) => r.Value;

        public override string ToString() => Value.ToString();
    }
}
