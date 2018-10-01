using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedMath.Calculus;
using RedMath.LinearAlgebra;
using RedMath.Structures;

namespace RedMathTests
{
    [TestClass]
    public class CalculusTests
    {
        [TestMethod]
        public void OptimizeSum()
        {
            Monomial mon = new Monomial(2, 1);
            AbsoluteValue abs = new AbsoluteValue();
            var sum = new FAddition<Real, Real, Real>(mon, abs);

            SGD.MinimizeFunctionValue(sum, 3, 1);
        }
    }
}
