using RedMath.Structures;
using RedMath.LinearAlgebra.MatrixOperations;
using System;
using System.Collections.Generic;

namespace RedMath.LinearAlgebra
{
    public class Matrix<T> where T : Field<T>, new()
    {
        #region Class Fields
        private T[,] _definingArray;

        private T _fieldZero = new T().Zero;
        private T _fieldOne = new T().One;
        #endregion

        #region Re-cache
        private bool r_CofactorMatrix = true;
        private bool r_Decomposition = true;
        private bool r_Determinant = true;
        private bool r_EchelonForm = true;
        private bool r_ReducedEchelonForm = true;
        private bool r_Identity = true;
        #endregion

        #region Cache Values
        private T _determinant;
        private List<MatrixOpertaion<T>> _echelonFormOperations;
        private List<MatrixOpertaion<T>> _reducedEchelonFormOperations;
        private Matrix<T> _echelonForm;
        private Matrix<T> _reducedEchelonForm;
        private Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>> _decomposition;
        private bool _identity;
        private Matrix<T> _cofactorMatrix;
        #endregion

        #region Size Properties
        public int Width
        {
            get
            {
                return _definingArray.GetLength(1);
            }
        }

        public int Height
        {
            get
            {
                return _definingArray.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return _definingArray.GetLength(1);
            }
        }

        public int Rows
        {
            get
            {
                return _definingArray.GetLength(0);
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
                    _identity = true;

                    if (!IsSquareMatrix)
                    {
                        _identity = false;
                    }

                    for (int row = 0; row < Height && _identity; row++)
                    {
                        for (int col = 0; col < Width && _identity; col++)
                        {
                            if (col == row && !_definingArray[row, col].Equals(_fieldOne))
                            {
                                _identity = false;
                            }
                            else if (col != row && !_definingArray[row, col].Equals(_fieldZero))
                            {
                                _identity = false;
                            }
                        }
                    }

                    r_Identity = false;
                }

                return _identity;
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

                for (int row = 0; row < Height; row++)
                {
                    for (int col = row + 1; col < Width; col++)
                    {
                        if (!_definingArray[row, col].Equals(_fieldZero))
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

                for (int row = 0; row < Height; row++)
                {
                    for (int col = 0; col < row; col++)
                    {
                        if (!_definingArray[row, col].Equals(_fieldZero))
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
                    seq[i] = _definingArray[i, i];
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
                    seq[i] = _definingArray[seq.Length - i - 1, i];
                }

                return seq;
            }
        }
        #endregion

        public Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>> Decomposition
        {
            get
            {
                if (r_Decomposition)
                {
                    _decomposition = computeDecomposition();
                    r_Decomposition = false;
                }

                return new Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>>(new Matrix<T>(_decomposition.Item1), new Matrix<T>(_decomposition.Item2), _decomposition.Item3);
            }
        }

        public T Determinant
        {
            get
            {
                if (r_Determinant)
                {
                    _determinant = computeDeterminant();
                    r_Determinant = false;
                }

                return _determinant;
            }
        }

        public List<MatrixOpertaion<T>> EchelonFormReductionOperations
        {
            get
            {
                if (r_EchelonForm)
                {
                    _echelonForm = computeEchelonForm();

                    r_EchelonForm = false;
                }

                return new List<MatrixOpertaion<T>>(_echelonFormOperations);
            }
        }

        public List<MatrixOpertaion<T>> ReducedEchelonFormReductionOperations
        {
            get
            {
                if (r_ReducedEchelonForm)
                {
                    _reducedEchelonForm = computeReducedEchelonForm();

                    r_ReducedEchelonForm = false;
                }

                return new List<MatrixOpertaion<T>>(_reducedEchelonFormOperations);
            }
        }

        public Matrix<T> EchelonForm
        {
            get
            {
                if (r_EchelonForm)
                {
                    _echelonForm = computeEchelonForm();

                    r_EchelonForm = false;
                }

                return new Matrix<T>(_echelonForm);
            }
        }

        public Matrix<T> ReducedEchelonForm
        {
            get
            {
                if (r_ReducedEchelonForm)
                {
                    _reducedEchelonForm = computeReducedEchelonForm();

                    r_ReducedEchelonForm = false;
                }

                return new Matrix<T>(_reducedEchelonForm);
            }
        }

        public Matrix<T> CofactorMatrix
        {
            get
            {
                if (r_CofactorMatrix)
                {
                    _cofactorMatrix = computeCofactorMatrix();
                    r_CofactorMatrix = false;
                }

                return new Matrix<T>(_cofactorMatrix);
            }
        }

        public Matrix<T> Inverse
        {
            get
            {
                if (!IsSquareMatrix || Determinant.Equals(_fieldZero))
                {
                    return null;
                }

                Matrix<T> mat = new Matrix<T>(Rows, Columns);
                for (int i = 0; i < Rows; i++) { mat[i, i] = new T().One; }

                List<MatrixOpertaion<T>> ops = ReducedEchelonFormReductionOperations;

                foreach (var op in ops)
                {
                    op.ApplyTo(mat);
                }

                return mat;
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
                return _definingArray[row, col];
            }

            set
            {
                _definingArray[row, col] = value;

                setCacheState(true);
            }
        }

        #region Constructors
        public Matrix()
        {
            _definingArray = new T[0, 0];
        }

        public Matrix(int rows, int cols)
        {
            _definingArray = new T[rows, cols];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _definingArray[i, j] = new T().Zero;
                }
            }
        }

        public Matrix(T[,] data)
        {
            this._definingArray = new T[data.GetLength(0), data.GetLength(1)];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    this._definingArray[row, col] = data[row, col];
                }
            }
        }

        public Matrix(Matrix<T> mat) : this(mat._definingArray) { }
        #endregion

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

        public void Resize(int rows, int cols)
        {
            if (rows == this.Rows && cols == this.Columns)
            {
                return;
            }

            Matrix<T> newData = new Matrix<T>(rows, cols);

            for (int i = 0; i < Math.Min(this.Rows, rows); i++)
            {
                for (int j = 0; j < Math.Min(this.Columns, cols); j++)
                {
                    newData[i, j] = _definingArray[i, j];
                }
            }

            this._definingArray = newData._definingArray;
        }

        public void Transpose()
        {
            T[,] temp = new T[Width, Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[j, i] = _definingArray[i, j];
                }
            }

            _definingArray = temp;

            setCacheState(true);
        }

        public Vector<T> GetRowVector(int index)
        {
            T[] res = new T[Width];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = _definingArray[index, i];
            }

            return new Vector<T>(res);
        }

        public Vector<T> GetColumnVector(int index)
        {
            T[] res = new T[Height];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = _definingArray[i, index];
            }

            return new Vector<T>(res);
        }

        public void AppendRowVector(Vector<T> row)
        {
            Matrix<T> temp = new Matrix<T>(Rows + 1, Columns);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = _definingArray[i, j];
                }
            }

            if (row.Dimension > temp.Width)
            {
                temp.Resize(temp.Rows, row.Dimension);
            }

            for (int i = 0; i < temp.Width; i++)
            {
                if (i < row.Dimension)
                {
                    temp[temp.Height - 1, i] = row[i];
                }
                else
                {
                    temp[temp.Height - 1, i] = new T().Zero;
                }
            }

            _definingArray = temp._definingArray;

            setCacheState(true);
        }

        public void InsertRowVector(Vector<T> row, int index)
        {
            Matrix<T> temp = new Matrix<T>(Rows + 1, Columns);
            int rowIndex = 0;

            for (int i = 0; i < temp.Height + 1; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = _definingArray[rowIndex, j];
                }

                rowIndex++;
            }

            if (row.Dimension > temp.Width)
            {
                temp.Resize(temp.Rows, row.Dimension);
            }

            for (int i = 0; i < temp.Width; i++)
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

            _definingArray = temp._definingArray;

            setCacheState(true);
        }

        public void AppendColumnVector(Vector<T> col)
        {
            Matrix<T> temp = new Matrix<T>(Rows, Columns + 1);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = _definingArray[i, j];
                }
            }

            if (col.Dimension > temp.Height)
            {
                temp.Resize(col.Dimension, temp.Columns);
            }

            for (int i = 0; i < temp.Height; i++)
            {
                if (i < col.Dimension)
                {
                    temp[i, temp.Columns - 1] = col[i];
                }
                else
                {
                    temp[i, temp.Columns - 1] = new T().Zero;
                }
            }

            _definingArray = temp._definingArray;

            setCacheState(true);
        }

        public void InsertColumnVector(Vector<T> col, int index)
        {
            Matrix<T> temp = new Matrix<T>(Rows, Columns + 1);
            int colIndex = 0;

            for (int i = 0; i < temp.Width; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Height; j++)
                {
                    temp[j, i] = _definingArray[j, colIndex];
                }

                colIndex++;
            }

            if (col.Dimension > temp.Height)
            {
                temp.Resize(col.Dimension, temp.Columns);
            }

            for (int i = 0; i < temp.Height; i++)
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

            _definingArray = temp._definingArray;

            setCacheState(true);
        }

        public T Minor(int row, int col)
        {
            return Submatrix(row, col).Determinant;
        }

        public T Cofactor(int row, int col)
        {
            return Minor(row, col).Multiply((row + col) % 2 == 0 ? _fieldOne : _fieldOne.AdditiveInverse);
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

        private Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>> computeDecomposition()
        {
            Matrix<T> U = new Matrix<T>(Math.Min(Rows, Columns), Columns);
            Matrix<T> L = new Matrix<T>(Rows, Math.Min(Rows, Columns));
            RowPermutation<T> P = new RowPermutation<T>();

            Matrix<T> temp = EchelonForm;

            for (int i = 0; i < U.Rows; i++)
            {
                for (int j = 0; j < U.Columns; j++)
                {
                    U[i, j] = temp[i, j];
                }
            }

            for (int i = 0; i < Math.Min(L.Rows, L.Columns); i++)
            {
                L[i, i] = new T().One;
            }

            List<MatrixOpertaion<T>> reductionOperations = EchelonFormReductionOperations;

            for (int i = reductionOperations.Count - 1; i >= 0; i--)
            {
                reductionOperations[i].InverseApplyTo(L);
            }

            int fullRows = 0;
            int zeroRows = 0;

            for (int i = 0; i < L.Height; i++)
            {
                int j = L.Width - 1;
                for (; j >= 0; j--)
                {
                    if (!L[i, j].Equals(_fieldZero))
                    {
                        break;
                    }
                }

                if (j == L.Width - 1)
                {
                    fullRows++;
                }
                else if (j == -1)
                {
                    zeroRows++;
                }
            }

            int fullOffset = L.Height - (fullRows + zeroRows);
            int zeroOffset = L.Height - zeroRows;

            int fullCount = 0;
            int zeroCount = 0;

            for (int i = 0; i < L.Height; i++)
            {
                int j = L.Width - 1;
                for (; j >= 0; j--)
                {
                    if (!L[i, j].Equals(_fieldZero))
                    {
                        break;
                    }
                }

                if (j == L.Width - 1)
                {
                    // Turn 'IndexList' to List<int>
                    P.IndexList.Add(fullOffset + fullCount);
                    fullCount++;
                }
                else if (j == -1)
                {
                    P.IndexList.Add(zeroOffset + zeroCount);
                    zeroCount++;
                }
                else
                {
                    P.IndexList.Add(j);
                }
            }

            P.ApplyTo(L);

            return new Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>>(L, U, P);
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


            RowPermutation<T> perm = Decomposition.Item3;

            int swaps = 1;

            /*for (int i = 0; i < perm.IndexList.Count; i++)
            {
                for (int j = i + 1; j < perm.IndexList.Count; j++)
                {
                    if (perm.IndexList[i] > perm.IndexList[j])
                    {
                        swaps *= -1;
                    }
                }
            }*/

            if (perm.IndexList[perm.IndexList.Count - 1] < perm.IndexList[perm.IndexList.Count - 2])
            {
                swaps *= -1;
            }

            if (swaps == 1)
            {
                return Utilitys.SequenceProduct(Decomposition.Item1.MainDiagonal).Multiply(Utilitys.SequenceProduct(Decomposition.Item2.MainDiagonal));
            }
            else
            {
                return _fieldOne.AdditiveInverse.Multiply(Utilitys.SequenceProduct(Decomposition.Item1.MainDiagonal).Multiply(Utilitys.SequenceProduct(Decomposition.Item2.MainDiagonal)));
            }
        }

        private Matrix<T> computeEchelonForm()
        {
            Matrix<T> temp = new Matrix<T>(this);
            List<MatrixOpertaion<T>> reductionOperations = new List<MatrixOpertaion<T>>();

            bool isZeroColumn = false;

            int rowOffset = 0;

            for (int col = 0; col < Math.Min(temp.Rows, temp.Columns); col++)
            {
                isZeroColumn = false;
                if (temp[col - rowOffset, col].Equals(_fieldZero))
                {
                    isZeroColumn = true;
                    for (int row = col + 1 - rowOffset; row < temp.Height; row++)
                    {
                        if (!temp[row, col].Equals(_fieldZero))
                        {
                            reductionOperations.Add(new SwapRows<T>(row, col - rowOffset));
                            reductionOperations[reductionOperations.Count - 1].ApplyTo(temp);
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

                reductionOperations.Add(new MultiplyRowByScalar<T>(col - rowOffset, temp[col - rowOffset, col].MultiplicativeInverse));
                reductionOperations[reductionOperations.Count - 1].ApplyTo(temp);

                for (int row = col + 1 - rowOffset; row < temp.Rows; row++)
                {
                    reductionOperations.Add(new AddRowMultiple<T>(col - rowOffset, row, temp[row, col].AdditiveInverse));
                    reductionOperations[reductionOperations.Count - 1].ApplyTo(temp);
                }
            }

            _echelonFormOperations = reductionOperations;

            return temp;
        }

        private Matrix<T> computeReducedEchelonForm()
        {
            Matrix<T> temp = EchelonForm;
            List<MatrixOpertaion<T>> reductionOperations = new List<MatrixOpertaion<T>>();

            bool isZeroRow = false;

            int entryIndex = 0;

            for (int baseRow = temp.Rows - 1; baseRow >= 0; baseRow--)
            {
                isZeroRow = true;
                for (int col = 0; col < temp.Columns; col++)
                {
                    if (temp[baseRow, col].Equals(_fieldOne))
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
                    reductionOperations.Add(new AddRowMultiple<T>(baseRow, currentRow, temp[currentRow, entryIndex].AdditiveInverse));
                    reductionOperations[reductionOperations.Count - 1].ApplyTo(temp);
                }
            }

            _reducedEchelonFormOperations = new List<MatrixOpertaion<T>>();
            _reducedEchelonFormOperations.AddRange(_echelonFormOperations);
            _reducedEchelonFormOperations.AddRange(reductionOperations);

            return temp;
        }

        private void setCacheState(bool state)
        {
            r_CofactorMatrix = state;
            r_Decomposition = state;
            r_Determinant = state;
            r_EchelonForm = state;
            r_ReducedEchelonForm = state;
            r_Identity = state;
        }

        #region Operators
        public static Matrix<T> operator +(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Width || left.Height != right.Height)
            {
                return null;
            }

            T[,] temp = new T[left.Height, left.Width];

            for (int i = 0; i < left.Height; i++)
            {
                for (int j = 0; j < left.Width; j++)
                {
                    temp[i, j] = left[i, j].Add(right[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Width || left.Height != right.Height)
            {
                return null;
            }

            T[,] temp = new T[left.Height, left.Width];

            for (int i = 0; i < left.Height; i++)
            {
                for (int j = 0; j < left.Width; j++)
                {
                    temp[i, j] = left[i, j].Subtract(right[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> value)
        {
            T[,] temp = new T[value.Height, value.Width];

            for (int i = 0; i < value.Height; i++)
            {
                for (int j = 0; j < value.Width; j++)
                {
                    temp[i, j] = value[i, j].AdditiveInverse;
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(Matrix<T> mat, T scalar)
        {
            T[,] temp = new T[mat.Height, mat.Width];

            for (int i = 0; i < mat.Height; i++)
            {
                for (int j = 0; j < mat.Width; j++)
                {
                    temp[i, j] = mat[i, j].Multiply(scalar);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(T scalar, Matrix<T> mat)
        {
            T[,] temp = new T[mat.Height, mat.Width];

            for (int i = 0; i < mat.Height; i++)
            {
                for (int j = 0; j < mat.Width; j++)
                {
                    temp[i, j] = mat[i, j].Multiply(scalar);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Height)
            {
                return null;
            }

            T[,] temp = new T[left.Height, right.Width];
            T sum = new T().Zero;

            for (int i = 0; i < temp.GetLength(0); i++)
            {
                for (int j = 0; j < temp.GetLength(1); j++)
                {
                    sum = new T().Zero;

                    for (int k = 0; k < left.Width; k++)
                    {
                        sum = sum.Add(left[i, k].Multiply(right[k, j]));
                    }

                    temp[i, j] = sum;
                }
            }

            return new Matrix<T>(temp);
        }

        public static implicit operator T[,] (Matrix<T> mat)
        {
            return (T[,])(mat._definingArray.Clone());
        }

        public static bool operator ==(Matrix<T> left, Matrix<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix<T> left, Matrix<T> right)
        {
            return !left.Equals(right);
        }
        #endregion

        #region Object Overrides
        public override bool Equals(object obj)
        {
            Matrix<T> other = obj as Matrix<T>;

            if (other == null)
            {
                return false;
            }

            if (Width != other.Width || Height != other.Height)
            {
                return false;
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (this[i, j] != other[i, j])
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
                    digits = this[i, j].ToString().Length;

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
                        digits = this[i, j].ToString().Length;

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