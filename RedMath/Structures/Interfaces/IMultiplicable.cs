using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public interface IMultiplicable<T> where T : IMultiplicable<T>
    {
        T Multiply(T other);
    }
}
