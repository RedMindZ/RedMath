using System;

namespace RedMath.LinearAlgebra
{
    public class Matrix
    {
        protected double[,] body;

        private bool r_Decomposition = true;
        private bool r_Determinant = true;
        private bool r_Identity = true;

        private double determinant;
        private Tuple<Matrix, Matrix> decomposition;
        private bool identity;

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

        public int Columns
        {
            get
            {
                return body.GetLength(1);
            }
        }

        public int Rows
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
                if (r_Identity)
                {
                    identity = true;

                    if (!IsSquareMatrix)
                    {
                        identity = false;
                    }

                    for (int row = 0; row < Height && identity; row++)
                    {
                        for (int col = 0; col < Width && identity; col++)
                        {
                            if (col == row && body[row, col] != 1)
                            {
                                identity = false;
                            }
                            else if (col != row && body[row, col] != 0)
                            {
                                identity = false;
                            }
                        }
                    }

                    r_Identity = false;
                }

                return identity;
            }
        }

        public bool IsLowerTriangular
        {
            get
            {
                if (!IsSquareMatrix)
                {
                    return false;
                }

                for (int i = 0; i < Height; i++)
                {
                    for (int j = i + 1; j < Width; j++)
                    {
                        if (this[i, j] != 0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public bool IsUpperTriangular
        {
            get
            {
                if (!IsSquareMatrix)
                {
                    return false;
                }

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (this[i, j] != 0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public double[] MainDiagonal
        {
            get
            {
                double[] seq = new double[(int)Algebra.Min(Height, Width)];

                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = this[i, i];
                }

                return seq;
            }
        }

        public double[] AntiDiagonal
        {
            get
            {
                double[] seq = new double[(int)Algebra.Min(Height, Width)];

                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = this[seq.Length - i - 1, i];
                }

                return seq;
            }
        }

        public Tuple<Matrix, Matrix> Decomposition
        {
            get
            {
                if (r_Decomposition)
                {
                    decomposition = decompose();
                    r_Decomposition = false;
                }

                return decomposition;
            }
        }

        public double Determinant
        {
            get
            {
                if (r_Determinant)
                {
                    determinant = computeDeterminant();
                    r_Determinant = false;
                }

                return determinant;
            }
        }

        public Matrix Transposition
        {
            get
            {
                Matrix m = new Matrix(this);
                m.Transpose();

                return m;
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

                r_Decomposition = true;
                r_Determinant = true;
                r_Identity = true;
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

            if (IsSquareMatrix)
            {
                for (int i = 0; i < Height; i++)
                {
                    body[i, i] = 1;
                }
            }
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

            r_Decomposition = true;
            r_Determinant = true;
            r_Identity = true;
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

        private Tuple<Matrix, Matrix> decompose()
        {
            if (!IsSquareMatrix)
            {
                return null;
            }

            Matrix lower = new Matrix(Rows, Columns);
            Matrix upper = new Matrix(Rows, Columns);

            for (int i = 0; i < body.GetLength(0); i++)
            {
                for (int j = i; j < body.GetLength(1); j++)
                {
                    upper[i, j] = body[i, j];
                    for (int k = 0; k < i; k++)
                    {
                        upper[i, j] = upper[i, j] - lower[i, k] * upper[k, j];
                    }
                }

                for (int j = i + 1; j < body.GetLength(1); j++)
                {
                    lower[j, i] = this[j, i];
                    for (int k = 0; k < i; k++)
                    {
                        lower[j, i] = lower[j, i] - lower[j, k] * upper[k, i];
                    }
                    lower[j, i] = lower[j, i] / upper[i, i];
                }
            }

            return new Tuple<Matrix, Matrix>(lower, upper);
        }

        private double computeDeterminant()
        {
            if (!IsSquareMatrix)
            {
                return Double.NaN;
            }

            return Utilitys.SequenceProduct(Decomposition.Item1.MainDiagonal) * Utilitys.SequenceProduct(Decomposition.Item2.MainDiagonal);
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

        public static implicit operator double[,] (Matrix m)
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
                    digits += body[row, col] < 0 ? 1 : 0;

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
