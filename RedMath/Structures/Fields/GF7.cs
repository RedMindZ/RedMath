using System.Collections.Generic;

namespace RedMath.Structures
{
    public class GF7 : Field<GF7>
    {
        private static readonly Dictionary<int, int> _inverseTable = new Dictionary<int, int> { { 1, 1 }, { 2, 4 }, { 3, 5 }, { 4, 2 }, { 5, 3 }, { 6, 6 } };

        public int Value { get; set; }

        protected override GF7 _zero => new GF7(0);
        protected override GF7 _one => new GF7(1);

        public GF7()
        {
            Value = 0;
        }

        public GF7(int value)
        {
            Value = value % 7;
        }


        public override GF7 AdditiveInverse => new GF7((7 - Value) % 7);
        public override GF7 MultiplicativeInverse => new GF7(_inverseTable[Value]);



        public override GF7 Add(GF7 other) => new GF7((Value + other.Value) % 7);
        public override GF7 Multiply(GF7 other) => new GF7((Value * other.Value) % 7);


        public override GF7 Clone() => new GF7(Value);
        public override bool Equals(GF7 other) => Value == other.Value;

        public override string ToString() => Value.ToString();
    }
}
