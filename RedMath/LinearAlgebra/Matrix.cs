using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.LinearAlgebra
{
    public class Matrix
    {
        protected double[,] body;

        public int Width
        {
            get
            {
                return body.GetLength(1);
            }
        }

        public int Height
        {
            get
            {
                return body.GetLength(0);
            }
        }

        public double this[int row, int col]
        {
            get
            {
                return body[row, col];
            }

            set
            {
                body[row, col] = value;
            }
        }

        public Matrix()
        {
            body = new double[1, 1];
            body[0, 0] = 1;
        }

        public Matrix(int rows, int cols)
        {
            body = new double[rows, cols];
        }

        public Matrix(double[,] data)
        {
            body = new double[data.GetLength(0), data.GetLength(1)];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    body[row, col] = data[row, col];
                }
            }
        }

        public override string ToString()
        {
            string res = "";
            int spaceCount = 0;
            int digits = 0;

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    digits = Algebra.CountDigits(body[row, col]);
                    if (digits > spaceCount)
                    {
                        spaceCount = digits;
                    }
                }
            }

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    res += body[row, col];

                    if (col < Width - 1)
                    {
                        digits = Algebra.CountDigits(body[row, col]);
                        for (int i = 0; i < spaceCount - digits + 1; i++)
                        {
                            res += " ";
                        }

                        res += " ";
                    }
                }

                if (row < Height - 1)
                {
                    res += "\n";
                }
            }

            return res;
        }
    }
}
