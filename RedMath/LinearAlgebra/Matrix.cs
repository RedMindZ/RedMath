using System;

namespace RedMath.LinearAlgebra
{
    public class Matrix
    {
        protected double[,] body;

        #region Re-cache
        private bool r_CofactorMatrix = true;
        private bool r_Decomposition = true;
        private bool r_Determinant = true;
        private bool r_Identity = true;
        #endregion

        #region Cache Values
        private double determinant;
        private Tuple<Matrix, Matrix, Matrix> decomposition;
        private bool identity;
        private Matrix cofactorMatrix;
        #endregion

        #region Size Properties
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
        #endregion

        #region Matrix Properties
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
        #endregion

        #region Diagonals
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
        #endregion

        public Tuple<Matrix, Matrix, Matrix> Decomposition
        {
            get
            {
                if (r_Decomposition)
                {
                    decomposition = computeDecomposition();
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

        public Matrix CofactorMatrix
        {
            get
            {
                if (r_CofactorMatrix)
                {
                    cofactorMatrix = computeCofactorMatrix();
                    r_CofactorMatrix = false;
                }

                return cofactorMatrix;
            }
        }

        public Matrix Inverse
        {
            get
            {
                if (!IsSquareMatrix || Determinant == 0)
                {
                    return null;
                }

                Matrix co = CofactorMatrix;

                return (1 / Determinant) * co.Transposition;
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

                r_CofactorMatrix = true;
                r_Decomposition = true;
                r_Determinant = true;
                r_Identity = true;
            }
        }

        #region Constructors
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
        #endregion

        public Matrix Submatrix(int row, int col)
        {
            int x = 0;
            int y = 0;

            double[,] temp = new double[Height - (row < 0 ? 0 : 1), Width - (col < 0 ? 0 : 1)];

            for (int i = 0; i < Height; i++)
            {
                if (i != row)
                {
                    x = 0;

                    for (int j = 0; j < Width; j++)
                    {
                        if (j != col)
                        {
                            temp[y, x] = this[i, j];

                            x++;
                        }
                    }

                    y++;
                }
            }

            return new Matrix(temp);
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

            r_CofactorMatrix = true;
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

        public void AddRowVector(Vector row)
        {
            double[,] temp = new double[Rows + 1, Columns];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = body[i, j];
                }
            }

            for (int i = 0; i < Width; i++)
            {
                if (i < row.Dimension)
                {
                    temp[Height, i] = row[i]; 
                }
                else
                {
                    break;
                }
            }

            body = temp;
        }

        public void AddColumnVector(Vector col)
        {
            double[,] temp = new double[Rows, Columns + 1];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = body[i, j];
                }
            }

            for (int i = 0; i < Height; i++)
            {
                if (i < col.Dimension)
                {
                    temp[i, Width] = col[i];
                }
                else
                {
                    break;
                }
            }

            body = temp;
        }

        public double Minor(int row, int col)
        {
            return Submatrix(row, col).Determinant;
        }

        public double Cofactor(int row, int col)
        {
            return Minor(row, col) * ((row + col) % 2 == 0 ? 1 : -1);
        }

        private Matrix computeCofactorMatrix()
        {
            if (!IsSquareMatrix)
            {
                return null;
            }

            Matrix m = new Matrix(Height, Width);

            for (int i = 0; i < m.Height; i++)
            {
                for (int j = 0; j < m.Width; j++)
                {
                    m[i, j] = Cofactor(i, j);
                }
            }

            return m;
        }

        private Tuple<Matrix, Matrix, Matrix> computeDecomposition()
        {
            Vector perm = new Vector(Height);

            double max = 0;
            int maxIndex = 0;
            double temp = 0;

            Matrix result = new Matrix(this);

            Matrix L;
            Matrix U;
            Matrix P = new Matrix(Height, Height);

            if (Width < Height)
            {
                U = new Matrix(Columns, Columns);
                L = new Matrix(Rows, Columns);

                for (int i = 0; i < L.Width; i++)
                {
                    L[i, i] = 1;
                }

                for (int i = 0; i < L.Height; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        L[i, j] = this[i, j];
                    }
                }
            }
            else if (Width > Height)
            {
                U = new Matrix(Rows, Columns);
                L = new Matrix(Rows, Rows);

                for (int i = 0; i < L.Height; i++)
                {
                    L[i, i] = 1;
                }

                for (int i = 0; i < U.Height; i++)
                {
                    for (int j = i; j < U.Width; j++)
                    {
                        U[i, j] = this[i, j];
                    }
                }
            }
            else
            {
                U = new Matrix(Rows, Columns);
                L = new Matrix(Rows, Columns);

                for (int i = 0; i < L.Height; i++)
                {
                    L[i, i] = 1;
                }

                for (int i = 0; i < U.Height; i++)
                {
                    for (int j = i; j < U.Width; j++)
                    {
                        U[i, j] = this[i, j];
                    }
                }
            }

            for (int i = 0; i < Height; i++)
            {
                perm[i] = i;
            }

            for (int i = 0; i < Height; i++)
            {
                max = 0;

                for (int j = i; j < Height; j++)
                {
                    if (Math.Abs(result[j, i]) > max)
                    {
                        max = Math.Abs(result[j, i]);
                        maxIndex = j;
                    }
                }

                if (max == 0)
                {
                    for (int j = 0; j < L.Height; j++)
                    {
                        for (int k = 0; k < j; k++)
                        {
                            L[j, k] = result[j, k];
                        }
                    }

                    for (int j = 0; j < U.Height; j++)
                    {
                        for (int k = j; k < U.Width; k++)
                        {
                            U[j, k] = result[j, k];
                        }
                    }

                    for (int j = 0; j < P.Height; j++)
                    {
                        P[j, j] = 0;

                        P[j, (int)perm[j]] = 1;
                    }

                    return new Tuple<Matrix, Matrix, Matrix>(P, L, U);
                }
                
                temp = perm[i];
                perm[i] = perm[maxIndex];
                perm[maxIndex] = temp;

                for (int j = 0; j < Height; j++)
                {
                    temp = result[i, j];
                    result[i, j] = result[maxIndex, j];
                    result[maxIndex, j] = temp;
                }
                
                for (int j = i + 1; j < Height; j++)
                {
                    result[j, i] /= result[i, i];
                    for (int k = i + 1; k < Height; k++)
                    {
                        result[j, k] -= result[j, i] * result[i, k];
                    }
                }
            }

            for (int i = 0; i < L.Height; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    L[i, j] = result[i, j];
                }
            }

            for (int i = 0; i < U.Height; i++)
            {
                for (int j = i; j < U.Width; j++)
                {
                    U[i, j] = result[i, j];
                }
            }

            for (int i = 0; i < P.Height; i++)
            {
                P[i, i] = 0;

                P[i, (int)perm[i]] = 1;
            }

            return new Tuple<Matrix, Matrix, Matrix>(P, L, U);
        }

        private double computeDeterminant()
        {
            if (!IsSquareMatrix)
            {
                return Double.NaN;
            }
            
            if (Height == 1)
            {
                return this[0, 0];
            }

            int swaps = 1;
            int index1 = 0;
            int index2 = 0;

            for (int i = Decomposition.Item1.Height - 2; i < Decomposition.Item1.Height; i++)
            {
                for (int j = 0; j < Decomposition.Item1.Width; j++)
                {
                    if (this[i, j] == 1 && i == Decomposition.Item1.Height - 2)
                    {
                        index1 = j;
                    }

                    if (this[i, j] == 1 && i == Decomposition.Item1.Height - 1)
                    {
                        index2 = j;
                    }
                }
            }

            swaps = index1 > index2 ? -1 : 1;

            return swaps * Utilitys.SequenceProduct(Decomposition.Item2.MainDiagonal) * Utilitys.SequenceProduct(Decomposition.Item3.MainDiagonal);
        }

        #region Operators
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
            return (double[,])m.body.Clone();
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
            {
                return false;
            }

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
            {
                return true;
            }

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Matrix other = obj as Matrix;

            if (other.Width != this.Width || other.Height != this.Height)
            {
                return false;
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (other[i, j] != this[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    sum += (int)this[i, j];
                }
            }

            return sum;
        }

        public override string ToString()
        {
            int digits = 0;
            int[] charCount = new int[Columns];

            bool[] negativeCols = new bool[Columns];
            bool fcHasNegatives = false;

            string res = "";

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    digits = Algebra.CountDigits(this[i, j]);

                    if (digits > charCount[j])
                    {
                        charCount[j] = digits;

                        if (this[i, j] < 0)
                        {
                            charCount[j]--;
                        }
                    }

                    if (this[i, j] < 0)
                    {
                        negativeCols[j] = true;
                    }
                }
            }

            fcHasNegatives = negativeCols[0];

            for (int j = 0; j < Columns; j++)
            {
                charCount[j] += 2;
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (negativeCols[j] && this[i, j] >= 0)
                    {
                        res += " ";
                    }

                    res += this[i, j];

                    if (j < Columns - 1)
                    {
                        digits = Algebra.CountDigits(this[i, j]);
                        digits -= this[i, j] < 0 ? 1 : 0;

                        for (int k = digits; k < charCount[j]; k++)
                        {
                            res += " ";
                        }
                    }
                }

                if (i < Rows - 1)
                {
                    res += '\n';
                }
            }

            return res;
        }
        #endregion
    }
}