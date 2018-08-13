using System.Collections.Generic;

namespace RedMath.Structures
{
    public class GF5 : Field<GF5>
    {
        private static Dictionary<int, int> _inverseTable = new Dictionary<int, int> { { 1, 1 }, { 2, 3 }, { 3, 2 }, { 4, 4 } };

        public int Value { get; set; }

        public override GF5 Zero => new GF5(0);
        public override GF5 One => new GF5(1);

        public GF5()
        {
            Value = 0;
        }

        public GF5(int value)
        {
            Value = value % 5;
        }


        public override GF5 AdditiveInverse => new GF5((5 - Value) % 5);
        public override GF5 MultiplicativeInverse => new GF5(_inverseTable[Value]);



        public override GF5 Add(GF5 other) => new GF5((Value + other.Value) % 5);
        public override GF5 Multiply(GF5 other) => new GF5((Value * other.Value) % 5);


        public override GF5 Clone() => new GF5(Value);
        public override bool Equals(GF5 other) => Value == other.Value;

        public override string ToString() => Value.ToString();
    }
}
