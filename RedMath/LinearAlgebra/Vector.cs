using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.LinearAlgebra
{
    public class Vector
    {
        double[] components;

        public int Dimension { get; private set; }

        public double Last
        {
            get
            {
                return components[components.Length - 1];
            }

            set
            {
                components[components.Length - 1] = value;
            }
        }

        public double this[int index]
        {
            get
            {
                return components[index];
            }

            set
            {
                components[index] = value;
            }
        }

        public Vector(int dim)
        {
            Dimension = dim;
            components = new double[Dimension];

            for (int i = 0; i < dim; i++)
            {
                components[i] = 0;
            }
        }

        public Vector(params double[] comp)
        {
            Dimension = comp.Length;
            components = new double[Dimension];

            for (int i = 0; i < comp.Length; i++)
            {
                components[i] = comp[i];
            }
        }


    }
}
