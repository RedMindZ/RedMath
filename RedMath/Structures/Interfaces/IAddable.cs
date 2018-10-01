using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Structures
{
    public interface IAddable<T> where T : IAddable<T>
    {
        T Add(T other);
    }
}
