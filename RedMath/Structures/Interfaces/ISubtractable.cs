using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public interface ISubtractable<T> where T : ISubtractable<T>
    {
        T Subtract(T other);
    }
}
