namespace RedMath.Structures
{
    public class Real : Field<Real>
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

                return null;
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

        public static Real operator +(Real a, Real b)
        {
            return new Real(a.Value + b.Value);
        }

        public static Real operator -(Real a, Real b)
        {
            return new Real(a.Value - b.Value);
        }

        public static Real operator *(Real a, Real b)
        {
            return new Real(a.Value * b.Value);
        }

        public static Real operator /(Real a, Real b)
        {
            return new Real(a.Value / b.Value);
        }

        public static bool operator ==(Real a, Real b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Real a, Real b)
        {
            return a.Value != b.Value;
        }

        public override Real Add(Real element)
        {
            return this + element;
        }

        public override Real Multiply(Real element)
        {
            return this * element;
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
            if (this.Value == other.Value)
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
    }
}
