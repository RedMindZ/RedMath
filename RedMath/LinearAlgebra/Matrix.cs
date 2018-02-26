using RedMath.Structures;
using RedMath.LinearAlgebra.MatrixOperations;
using RedMath.Utils;

using System;
using System.Collections.Generic;

namespace RedMath.LinearAlgebra
{
    public class Matrix<T> where T : Field<T>, new()
    {
        #region Class Fields
        private T[,] definingArray;

        private static T fieldZero = new T().Zero;
        private static T fieldOne = new T().One;
        #endregion

        protected Cache<string> computationCache = new Cache<string>();

        /*
        #region Re-cache
        private bool r_Determinant = true;
        private bool r_EchelonForm = true;
        private bool r_ReducedEchelonForm = true;
        private bool r_Decomposition = true;
        private bool r_Identity = true;
        private bool r_CofactorMatrix = true;
        #endregion

        #region Cache Values
        private T _determinant;
        private List<MatrixOperation<T>> _echelonFormOperations;
        private List<MatrixOperation<T>> reducedEchelonFormOperations;
        private Matrix<T> _echelonForm;
        private Matrix<T> _reducedEchelonForm;
        private Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>> _decomposition;
        private bool _identity;
        private Matrix<T> _cofactorMatrix;
        #endregion
        */
        #region Size Properties
        public int Width
        {
            get
            {
                return definingArray.GetLength(1);
            }
        }

        public int Height
        {
            get
            {
                return definingArray.GetLength(0);
            }
        }

        public int Columns
        {
            get
            {
                return definingArray.GetLength(1);
            }
        }

        public int Rows
        {
            get
            {
                return definingArray.GetLength(0);
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
                /*if (r_Identity)
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
                            if (col == row && !definingArray[row, col].Equals(fieldOne))
                            {
                                _identity = false;
                            }
                            else if (col != row && !definingArray[row, col].Equals(fieldZero))
                            {
                                _identity = false;
                            }
                        }
                    }

                    r_Identity = false;
                }*/

                return computationCache.RetrieveValue<bool, Matrix<T>>("identity", this);
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
                        if (!definingArray[row, col].Equals(fieldZero))
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
                        if (!definingArray[row, col].Equals(fieldZero))
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
                T[] seq = new T[Math.Min(Height, Width)];

                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = definingArray[i, i].Clone();
                }

                return seq;
            }
        }

        public T[] AntiDiagonal
        {
            get
            {
                T[] seq = new T[Math.Min(Height, Width)];

                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = definingArray[seq.Length - i - 1, i].Clone();
                }

                return seq;
            }
        }
        #endregion

        public LUPDecomposition<T> Decomposition
        {
            get
            {
                /*if (r_Decomposition)
                {
                    _decomposition = computeDecomposition();
                    r_Decomposition = false;
                }*/

                return new LUPDecomposition<T>(computationCache.RetrieveValue<LUPDecomposition<T>, Matrix<T>>("decomposition", this));
            }
        }

        public T Determinant
        {
            get
            {
                /*
                if (r_Determinant)
                {
                    _determinant = computeDeterminant();
                    r_Determinant = false;
                }

                return _determinant;*/

                return computationCache.RetrieveValue<T, Matrix<T>>("determinant", this);
            }
        }

        public List<MatrixOperation<T>> EchelonFormReductionOperations
        {
            get
            {
                /*
                if (r_EchelonForm)
                {
                    _echelonForm = computeEchelonForm().Item1;

                    r_EchelonForm = false;
                }

                return new List<MatrixOperation<T>>(_echelonFormOperations);*/

                return new List<MatrixOperation<T>>(computationCache.RetrieveValue<List<MatrixOperation<T>>, Matrix<T>>("echelonFormOperations", this));
            }
        }

        public List<MatrixOperation<T>> ReducedEchelonFormReductionOperations
        {
            get
            {
                /*if (r_ReducedEchelonForm)
                {
                    _reducedEchelonForm = computeReducedEchelonForm().Item1;

                    r_ReducedEchelonForm = false;
                }

                return new List<MatrixOperation<T>>(reducedEchelonFormOperations);*/

                return new List<MatrixOperation<T>>(computationCache.RetrieveValue<List<MatrixOperation<T>>, Matrix<T>>("reducedEchelonFormOperations", this));
            }
        }

        public Matrix<T> EchelonForm
        {
            get
            {
                /*if (r_EchelonForm)
                {
                    _echelonForm = computeEchelonForm().Item1;

                    r_EchelonForm = false;
                }

                return new Matrix<T>(_echelonForm);*/

                return new Matrix<T>(computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("echelonForm", this));
            }
        }

        public Matrix<T> ReducedEchelonForm
        {
            get
            {
                /*if (r_ReducedEchelonForm)
                {
                    _reducedEchelonForm = computeReducedEchelonForm().Item1;

                    r_ReducedEchelonForm = false;
                }

                return new Matrix<T>(_reducedEchelonForm);*/

                return new Matrix<T>(computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("reducedEchelonForm", this));
            }
        }

        public Matrix<T> CofactorMatrix
        {
            get
            {
                /*
                if (r_CofactorMatrix)
                {
                    _cofactorMatrix = computeCofactorMatrix();
                    r_CofactorMatrix = false;
                }

                return new Matrix<T>(_cofactorMatrix);*/

                return new Matrix<T>(computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("cofactorMatrix", this));
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

                Matrix<T> mat = new Matrix<T>(Rows, Columns);
                for (int i = 0; i < Rows; i++) { mat[i, i] = new T().One; }

                List<MatrixOperation<T>> ops = ReducedEchelonFormReductionOperations;

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
                return definingArray[row, col].Clone();
            }

            set
            {
                definingArray[row, col] = value.Clone();

                computationCache.SetAllUpdateStates(true);
            }
        }

        #region Constructors
        public Matrix() : this(0, 0) { }

        public Matrix(int rows, int cols)
        {
            definingArray = new T[rows, cols];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    definingArray[i, j] = fieldZero.Clone();
                }
            }

            initCache();
        }

        public Matrix(T[,] data)
        {
            definingArray = new T[data.GetLength(0), data.GetLength(1)];

            for (int row = 0; row < data.GetLength(0); row++)
            {
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    definingArray[row, col] = data[row, col].Clone();
                }
            }

            initCache();
        }

        public Matrix(Matrix<T> mat) : this(mat.definingArray) { }

        private void initCache()
        {
            computationCache.AddCacheEntry("determinant", null, true, (Matrix<T> mat) => mat.computeDeterminant());
            computationCache.AddCacheEntry("echelonForm", null, true, (Matrix<T> mat) => mat.computeEchelonForm().Item1);
            computationCache.AddCacheEntry("reducedEchelonForm", null, true, (Matrix<T> mat) => mat.computeReducedEchelonForm().Item1);
            computationCache.AddCacheEntry("echelonFormOperations", new List<MatrixOperation<T>>(), true, (Matrix<T> mat) => mat.computeEchelonForm().Item2);
            computationCache.AddCacheEntry("reducedEchelonFormOperations", new List<MatrixOperation<T>>(), true, (Matrix<T> mat) => mat.computeReducedEchelonForm().Item2);
            computationCache.AddCacheEntry("decomposition", null, true, (Matrix<T> mat) => mat.computeDecomposition());
            computationCache.AddCacheEntry("identity", false, true, (Matrix<T> mat) => mat.testForIdentity());
            computationCache.AddCacheEntry("cofactorMatrix", null, true, (Matrix<T> mat) => mat.computeCofactorMatrix());
        }
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
            if (rows == Rows && cols == Columns)
            {
                return;
            }

            T[,] newData = new T[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i < Rows && j < Columns)
                    {
                        newData[i, j] = definingArray[i, j];
                    }
                    else
                    {
                        newData[i, j] = fieldZero.Clone();
                    }
                }
            }

            definingArray = newData;

            computationCache.SetAllUpdateStates(true);
        }

        public void Transpose()
        {
            T[,] temp = new T[Width, Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[j, i] = definingArray[i, j];
                }
            }

            definingArray = temp;

            computationCache.SetAllUpdateStates(true);
        }

        public Vector<T> GetRowVector(int index)
        {
            T[] res = new T[Width];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = definingArray[index, i].Clone();
            }

            return new Vector<T>(res);
        }

        public Vector<T> GetColumnVector(int index)
        {
            T[] res = new T[Height];

            for (int i = 0; i < res.Length; i++)
            {
                res[i] = definingArray[i, index].Clone();
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
                    temp[i, j] = definingArray[i, j];
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
                    temp[temp.Height - 1, i] = fieldZero.Clone();
                }
            }

            definingArray = temp.definingArray;

            computationCache.SetAllUpdateStates(true);
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
                    temp[i, j] = definingArray[rowIndex, j];
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
                    temp[index, i] = fieldZero.Clone();
                }
            }

            definingArray = temp.definingArray;

            computationCache.SetAllUpdateStates(true);
        }

        public void AppendColumnVector(Vector<T> col)
        {
            Matrix<T> temp = new Matrix<T>(Rows, Columns + 1);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp[i, j] = definingArray[i, j];
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
                    temp[i, temp.Columns - 1] = fieldZero.Clone();
                }
            }

            definingArray = temp.definingArray;

            computationCache.SetAllUpdateStates(true);
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
                    temp[j, i] = definingArray[j, colIndex];
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
                    temp[i, index] = fieldZero.Clone();
                }
            }

            definingArray = temp.definingArray;

            computationCache.SetAllUpdateStates(true);
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

        private LUPDecomposition<T> computeDecomposition()
        {
            /*Matrix<T> lowerMat = new Matrix<T>(Rows, Math.Min(Rows, Columns));
            Matrix<T> upperMat = new Matrix<T>(Math.Min(Rows, Columns), Columns);
            RowPermutation<T> perm = new RowPermutation<T>();

            Matrix<T> temp = EchelonForm;

            for (int i = 0; i < upperMat.Rows; i++)
            {
                for (int j = 0; j < upperMat.Columns; j++)
                {
                    upperMat[i, j] = temp[i, j];
                }
            }

            for (int i = 0; i < Math.Min(lowerMat.Rows, lowerMat.Columns); i++)
            {
                lowerMat[i, i] = fieldOne.Clone();
            }

            List<MatrixOperation<T>> reductionOperations = EchelonFormReductionOperations;

            for (int i = reductionOperations.Count - 1; i >= 0; i--)
            {
                reductionOperations[i].InverseApplyTo(lowerMat);
            }

            int fullRows = 0;
            int zeroRows = 0;

            for (int i = 0; i < lowerMat.Height; i++)
            {
                int j = lowerMat.Width - 1;
                for (; j >= 0; j--)
                {
                    if (!lowerMat[i, j].Equals(fieldZero))
                    {
                        break;
                    }
                }

                if (j == lowerMat.Width - 1)
                {
                    fullRows++;
                }
                else if (j == -1)
                {
                    zeroRows++;
                }
            }

            int fullOffset = lowerMat.Height - (fullRows + zeroRows);
            int zeroOffset = lowerMat.Height - zeroRows;

            int fullCount = 0;
            int zeroCount = 0;

            for (int i = 0; i < lowerMat.Height; i++)
            {
                int j = lowerMat.Width - 1;
                for (; j >= 0; j--)
                {
                    if (!lowerMat[i, j].Equals(fieldZero))
                    {
                        break;
                    }
                }

                if (j == lowerMat.Width - 1)
                {
                    // Turn 'IndexList' to List<int>
                    perm.IndexList.Add(fullOffset + fullCount);
                    fullCount++;
                }
                else if (j == -1)
                {
                    perm.IndexList.Add(zeroOffset + zeroCount);
                    zeroCount++;
                }
                else
                {
                    perm.IndexList.Add(j);
                }
            }

            perm.ApplyTo(lowerMat);

            return new Tuple<Matrix<T>, Matrix<T>, RowPermutation<T>>(lowerMat, upperMat, perm);*/

            return new LUPDecomposition<T>(this);
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

            T det = Utilitys.SequenceProduct(Decomposition.LowerMatrix.MainDiagonal).Multiply(Utilitys.SequenceProduct(Decomposition.UpperMatrix.MainDiagonal));
            
            if (Decomposition.Permutation.Signature == 1)
            {
                return det;
            }
            else
            {
                return det.AdditiveInverse;
            }
        }

        private Tuple<Matrix<T>, List<MatrixOperation<T>>> computeEchelonForm()
        {
            Matrix<T> temp = new Matrix<T>(this);
            List<MatrixOperation<T>> reductionOperations = new List<MatrixOperation<T>>();

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

            return new Tuple<Matrix<T>, List<MatrixOperation<T>>>(temp, reductionOperations);
        }

        private Tuple<Matrix<T>, List<MatrixOperation<T>>> computeReducedEchelonForm()
        {
            Matrix<T> reducedEchelonFormMatrix = EchelonForm;
            List<MatrixOperation<T>> reductionOperations = new List<MatrixOperation<T>>();

            bool isZeroRow = false;

            int entryIndex = 0;

            for (int baseRow = reducedEchelonFormMatrix.Rows - 1; baseRow >= 0; baseRow--)
            {
                isZeroRow = true;
                for (int col = 0; col < reducedEchelonFormMatrix.Columns; col++)
                {
                    if (reducedEchelonFormMatrix[baseRow, col].Equals(fieldOne))
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
                    reductionOperations.Add(new AddRowMultiple<T>(baseRow, currentRow, reducedEchelonFormMatrix[currentRow, entryIndex].AdditiveInverse));
                    reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedEchelonFormMatrix);
                }
            }

            List<MatrixOperation<T>> reducedEchelonFormOperations = new List<MatrixOperation<T>>();
            reducedEchelonFormOperations.AddRange(EchelonFormReductionOperations);
            reducedEchelonFormOperations.AddRange(reductionOperations);

            return new Tuple<Matrix<T>, List<MatrixOperation<T>>>(reducedEchelonFormMatrix, reducedEchelonFormOperations);
        }

        private bool testForIdentity()
        {
            bool isIdentity = true;

            if (!IsSquareMatrix)
            {
                isIdentity = false;
            }

            for (int row = 0; row < Height && isIdentity; row++)
            {
                for (int col = 0; col < Width && isIdentity; col++)
                {
                    if (col == row && !definingArray[row, col].Equals(fieldOne))
                    {
                        isIdentity = false;
                        break;
                    }
                    else if (col != row && !definingArray[row, col].Equals(fieldZero))
                    {
                        isIdentity = false;
                        break;
                    }
                }

                if (!isIdentity)
                {
                    break;
                }
            }

            return isIdentity;
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
            return (T[,])(mat.definingArray.Clone());
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

            if ((object)other == null)
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