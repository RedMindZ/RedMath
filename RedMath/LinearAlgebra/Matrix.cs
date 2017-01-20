using RedMath.Structures;
using RedMath.LinearAlgebra.MatrixOperations;
using System;

namespace RedMath.LinearAlgebra
{
    public class Matrix<T> where T : Field<T>, new()
    {
        private T[,] data;

        protected T fieldZero = new T().Zero;
        protected T fieldOne = new T().One;

        #region Re-cache
        private bool r_CofactorMatrix = true;
        private bool r_Decomposition = true;
        private bool r_Determinant = true;
        private bool r_EchelonForm = true;
        private bool r_reducedEchelonForm = true;
        private bool r_Identity = true;
        #endregion

        #region Cache Values
        private T determinant;
        private Matrix<T> echelonForm;
        private Matrix<T> reducedEchelonForm;
        private Tuple<Matrix<T>, Matrix<T>, Matrix<T>> decomposition;
        private bool identity;
        private Matrix<T> cofactorMatrix;
        #endregion

        #region Size Properties
        public int Width
        {
            get
            {
                return data.GetLength(1);
            }
        }

        public int Height
        {
            get
            {
                return data.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return data.GetLength(1);
            }
        }

        public int Rows
        {
            get
            {
                return data.GetLength(0);
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
                            if (col == row && data[row, col].Equals(fieldOne))
                            {
                                identity = false;
                            }
                            else if (col != row && !data[row, col].Equals(fieldZero))
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
                        if (!this[i, j].Equals(fieldZero))
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
                        if (!this[i, j].Equals(fieldZero))
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
        public T[] MainDiagonal
        {
            get
            {
                T[] seq = new T[(int)Math.Min(Height, Width)];

                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = this[i, i];
                }

                return seq;
            }
        }

        public T[] AntiDiagonal
        {
            get
            {
                T[] seq = new T[(int)Math.Min(Height, Width)];

                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = this[seq.Length - i - 1, i];
                }

                return seq;
            }
        }
        #endregion


        public Tuple<Matrix<T>, Matrix<T>, Matrix<T>> Decomposition
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

        public T Determinant
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

        public Matrix<T> EchelonForm
        {
            get
            {
                if (r_EchelonForm)
                {
                    echelonForm = computeEchelonForm();

                    r_EchelonForm = false;
                }

                return new Matrix<T>(echelonForm);
            }
        }

        public Matrix<T> ReducedEchelonForm
        {
            get
            {
                if (r_reducedEchelonForm)
                {
                    reducedEchelonForm = computeReducedEchelonForm();

                    r_reducedEchelonForm = false;
                }

                return new Matrix<T>(reducedEchelonForm);
            }
        }

        public Matrix<T> CofactorMatrix
        {
            get
            {
                if (r_CofactorMatrix)
                {
                    cofactorMatrix = computeCofactorMatrix();
                    r_CofactorMatrix = false;
                }

                return new Matrix<T>(cofactorMatrix);
            }
        }

        public Matrix<T> Inverse
        {
            get
            {
                if (!IsSquareMatrix || Determinant.Equals(fieldZero))
                {
                    return null;
                }

                Matrix<T> co = CofactorMatrix;

                return (Determinant.MultiplicativeInverse) * co.Transposition;
            }
        }

        public Matrix<T> Transposition
        {
            get
            {
                Matrix<T> m = new Matrix<T>(this);
                m.Transpose();

                return m;
            }
        }

        public T this[int row, int col]
        {
            get
            {
                return data[row, col];
            }

            set
            {
                data[row, col] = value;

                setCacheState(true);
            }
        }

        #region Constructors
        public Matrix()
        {
            data = new T[1, 1];
            data[0, 0] = new T().One;
        }

        public Matrix(int rows, int cols)
        {
            data = new T[rows, cols];

            if (IsSquareMatrix)
            {
                for (int i = 0; i < Height; i++)
                {
                    data[i, i] = new T().One;
                }
            }
        }

        public Matrix(T[,] data)
        {
            this.data = new T[data.GetLength(0), data.GetLength(1)];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    this.data[row, col] = data[row, col];
                }
            }
        }

        public Matrix(Matrix<T> mat)
        {
            data = new T[mat.Rows, mat.Columns];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    data[row, col] = mat[row, col];
                }
            }
        }
        #endregion

        private void setCacheState(bool state)
        {
            r_CofactorMatrix = state;
            r_Decomposition = state;
            r_Determinant = state;
            r_EchelonForm = state;
            r_reducedEchelonForm = state;
            r_Identity = state;
        }

        public Matrix<T> Submatrix(int row, int col)
        {
            int x = 0;
            int y = 0;

            T[,] temp = new T[Height - (row < 0 ? 0 : 1), Width - (col < 0 ? 0 : 1)];

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

            return new Matrix<T>(temp);
        }

        public void Transpose()
        {
            T[,] temp = new T[Width, Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[j, i] = data[i, j];
                }
            }

            data = temp;

            setCacheState(true);
        }

        public Vector<T> GetRowVector(int index)
        {
            T[] res = new T[Width];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = data[index, i];
            }

            return new Vector<T>(res);
        }

        public Vector<T> GetColumnVector(int index)
        {
            T[] res = new T[Height];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = data[i, index];
            }

            return new Vector<T>(res);
        }

        public void AppendRowVector(Vector<T> row)
        {
            T[,] temp = new T[Rows + 1, Columns];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = data[i, j];
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
                    temp[Height, i] = new T().Zero;
                }
            }

            data = temp;

            setCacheState(true);
        }

        public void InsertRowVector(Vector<T> row, int index)
        {
            T[,] temp = new T[Rows + 1, Columns];
            int rowIndex = 0;

            for (int i = 0; i < Height + 1; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = data[rowIndex, j];
                    rowIndex++;
                }
            }

            for (int i = 0; i < Width; i++)
            {
                if (i < row.Dimension)
                {
                    temp[index, i] = row[i];
                }
                else
                {
                    temp[index, i] = new T().Zero;
                }
            }

            data = temp;

            setCacheState(true);
        }

        public void AppendColumnVector(Vector<T> col)
        {
            T[,] temp = new T[Rows, Columns + 1];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = data[i, j];
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
                    temp[i, Width] = new T().Zero;
                }
            }

            data = temp;

            setCacheState(true);
        }

        public void InsertColumnVector(Vector<T> col, int index)
        {
            T[,] temp = new T[Rows, Columns + 1];
            int colIndex = 0;

            for (int i = 0; i < Width; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Height; j++)
                {
                    temp[j, i] = data[j, colIndex];
                    colIndex++;
                }
            }

            for (int i = 0; i < Height; i++)
            {
                if (i < col.Dimension)
                {
                    temp[i, index] = col[i];
                }
                else
                {
                    temp[i, index] = new T().Zero;
                }
            }

            data = temp;

            setCacheState(true);
        }

        public T Minor(int row, int col)
        {
            return Submatrix(row, col).Determinant;
        }

        public T Cofactor(int row, int col)
        {
            return Minor(row, col).Multiply((row + col) % 2 == 0 ? fieldOne : fieldOne.AdditiveInverse);
        }

        private Matrix<T> computeCofactorMatrix()
        {
            if (!IsSquareMatrix)
            {
                return null;
            }

            Matrix<T> m = new Matrix<T>(Height, Width);

            for (int i = 0; i < m.Height; i++)
            {
                for (int j = 0; j < m.Width; j++)
                {
                    m[i, j] = Cofactor(i, j);
                }
            }

            return m;
        }

        private Tuple<Matrix<T>, Matrix<T>, Matrix<T>> computeDecomposition()
        {
            return null;
        }

        private T computeDeterminant()
        {
            if (!IsSquareMatrix)
            {
                return null;
            }

            if (Height == 1)
            {
                return this[0, 0];
            }

            T swaps;
            int index1 = 0;
            int index2 = 0;

            for (int i = 0; i < Decomposition.Item1.Width; i++)
            {
                if (this[Decomposition.Item1.Height - 2, i].Equals(fieldOne))
                {
                    index1 = i;
                }
            }

            for (int j = 0; j < Decomposition.Item1.Width; j++)
            {
                if (this[Decomposition.Item1.Height - 1, j].Equals(fieldOne))
                {
                    index2 = j;
                }
            }

            swaps = index1 > index2 ? fieldOne.AdditiveInverse : fieldOne;

            return swaps.Multiply(Utilitys.SequenceProduct(Decomposition.Item2.MainDiagonal).Multiply(Utilitys.SequenceProduct(Decomposition.Item3.MainDiagonal)));
        }

        public Matrix<T> computeEchelonForm()
        {
            Matrix<T> temp = new Matrix<T>(this);

            bool isZeroColumn = false;

            int rowOffset = 0;

            for (int col = 0; col < Math.Min(temp.Rows, temp.Columns); col++)
            {
                isZeroColumn = false;
                if (temp[col - rowOffset, col].Equals(fieldZero))
                {
                    isZeroColumn = true;
                    for (int row = col + 1 - rowOffset; row < temp.Height; row++)
                    {
                        if (!temp[row, col].Equals(fieldZero))
                        {
                            new SwapRows<T>(row, col).ApplyTo(temp);
                            isZeroColumn = false;
                            break;
                        }
                    }
                }

                if (isZeroColumn)
                {
                    rowOffset++;
                    continue;
                }

                new MultiplyRowByScalar<T>(col - rowOffset, temp[col - rowOffset, col].MultiplicativeInverse).ApplyTo(temp);

                for (int row = col + 1 - rowOffset; row < temp.Rows; row++)
                {
                    new AddRowMultiple<T>(col - rowOffset, row, temp[row, col].AdditiveInverse).ApplyTo(temp);
                }
            }

            return temp;
        }

        public Matrix<T> computeReducedEchelonForm()
        {
            Matrix<T> temp = EchelonForm;

            bool isZeroRow = false;

            int entryIndex = 0;

            for (int baseRow = temp.Rows - 1; baseRow >= 0; baseRow--)
            {
                isZeroRow = true;
                for (int col = 0; col < temp.Rows; col++)
                {
                    if (temp[baseRow, col].Equals(fieldOne))
                    {
                        entryIndex = col;
                        isZeroRow = false;
                        break;
                    }
                }

                if (isZeroRow)
                {
                    continue;
                }

                for (int currentRow = 0; currentRow < baseRow; currentRow++)
                {
                    new AddRowMultiple<T>(baseRow, currentRow, temp[currentRow, entryIndex].AdditiveInverse).ApplyTo(temp);
                }
            }

            return temp;
        }

        #region Operators
        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
            {
                return null;
            }

            T[,] temp = new T[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j].Add(b[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> a, Matrix<T> b)
        {
            if (a.Width != b.Width || a.Height != b.Height)
            {
                return null;
            }

            T[,] temp = new T[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j].Subtract(b[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> a)
        {
            T[,] temp = new T[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j].AdditiveInverse;
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(Matrix<T> a, T s)
        {
            T[,] temp = new T[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j].Multiply(s);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(T s, Matrix<T> a)
        {
            T[,] temp = new T[a.Height, a.Width];

            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    temp[i, j] = a[i, j].Multiply(s);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            if (a.Width != b.Height)
            {
                return null;
            }

            T[,] temp = new T[a.Height, b.Width];
            T sum = new T().Zero;

            for (int i = 0; i < temp.GetLength(0); i++)
            {
                for (int j = 0; j < temp.GetLength(1); j++)
                {
                    sum = new T().Zero;

                    for (int k = 0; k < a.Width; k++)
                    {
                        sum = sum.Add(a[i, k].Multiply(b[k, j]));
                    }

                    temp[i, j] = sum;
                }
            }

            return new Matrix<T>(temp);
        }

        public static implicit operator T[,] (Matrix<T> m)
        {
            return (T[,])(m.data.Clone());
        }

        public static bool operator ==(Matrix<T> a, Matrix<T> b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }

            if ((object)a == null ^ (object)b == null)
            {
                return false;
            }

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

        public static bool operator !=(Matrix<T> a, Matrix<T> b)
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

            Matrix<T> other = obj as Matrix<T>;

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
                    sum += this[i, j].GetHashCode();
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
                    digits = Utilitys.CountDigits(this[i, j]);

                    if (digits > charCount[j])
                    {
                        charCount[j] = digits;
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
                    res += this[i, j];

                    if (j < Columns - 1)
                    {
                        digits = Utilitys.CountDigits(this[i, j]);

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