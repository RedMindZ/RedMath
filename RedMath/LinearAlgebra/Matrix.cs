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

        public bool IsColumnMatrix
        {
            get
            {
                return Width == 1;
            }
        }

        public bool IsRowMatrix
        {
            get
            {
                return Height == 1;
            }
        }

        public bool IsSquareMatrix
        {
            get
            {
                return Width == Height;
            }
        }

        public bool IsIdentity
        {
            get
            {
                if (!IsSquareMatrix)
                {
                    return false;
                }

                for (int row = 0; row < Height; row++)
                {
                    for (int col = 0; col < Width; col++)
                    {
                        if (col == row && body[row, col] != 1)
                        {
                            return false;
                        }
                        else if (col != row && body[row, col] != 0)
                        {
                            return false;
                        }
                    }
                }

                return true;
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

        public void Transpose()
        {
            double[,] temp = new double[Width, Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[j, i] = body[i, j];
                }
            }

            body = temp;
        }

        public Vector GetRowVector(int index)
        {
            double[] res = new double[Width];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = body[index, i];
            }

            return new Vector(res);
        }

        public Vector GetColumnVector(int index)
        {
            double[] res = new double[Height];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = body[i, index];
            }

            return new Vector(res);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
            {
                return null;
            }

            double[,] temp = new double[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j] + b[i, j];
                }
            }

            return new Matrix(temp);
        }

        public static Matrix operator *(Matrix a, double s)
        {
            double[,] temp = new double[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j] * s;
                }
            }

            return new Matrix(temp);
        }

        public static Matrix operator *(double s, Matrix a)
        {
            double[,] temp = new double[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j] * s;
                }
            }

            return new Matrix(temp);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Width != b.Height)
            {
                return null;
            }

            double[,] temp = new double[a.Height, b.Width];
            double sum = 0;

            for (int i = 0; i < temp.GetLength(0); i++)
            {
                for (int j = 0; j < temp.GetLength(1); j++)
                {
                    sum = 0;

                    for (int k = 0; k < a.Width; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }

                    temp[i, j] = sum;
                }
            }

            return new Matrix(temp);
        }

        public static implicit operator double[,](Matrix m)
        {
            return m.body;
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
