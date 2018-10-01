using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public interface IDivisible<T> where T : IDivisible<T>
    {
        T Divide(T other);
    }
}
