using Alea;
using Alea.CSharp;
using Alea.Parallel;
using RedMath.GpuUtils;
using RedMath.LinearAlgebra.MatrixOperations;
using RedMath.Structures;
using RedMath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedMath.LinearAlgebra
{
    public class Matrix<T> where T : Field<T>, new()
    {
        #region Class Fields
        [GpuParam]
        protected T[,] definingArray;

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
                /*
                if (!IsSquareMatrix)
                {
                    throw new InvalidOperationException("A non-square matrix doesn't have an inverse.");
                }
                else if (Determinant.Equals(fieldZero))
                {
                    throw new InvalidOperationException("A matrix with determinant 0 doesn't have an inverse.");
                }

                Matrix<T> mat = new Matrix<T>(Rows, Columns);
                for (int i = 0; i < Rows; i++) { mat[i, i] = new T().One; }

                List<MatrixOperation<T>> ops = ReducedEchelonFormReductionOperations;

                foreach (var op in ops)
                {
                    op.ApplyTo(mat);
                }

                return mat;*/

                return computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("inverse", this);
            }
        }

        public int Rank
        {
            get
            {
                return computationCache.RetrieveValue<int, Matrix<T>>("rank", this);
            }
        }

        public bool IsFullRank
        {
            get
            {
                return IsSquareMatrix && Rank == Rows;
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
            computationCache.AddCacheEntry("echelonFormOperations", null, true, (Matrix<T> mat) => mat.computeEchelonForm().Item2);
            computationCache.AddCacheEntry("reducedEchelonFormOperations", null, true, (Matrix<T> mat) => mat.computeReducedEchelonForm().Item2);
            computationCache.AddCacheEntry("decomposition", null, true, (Matrix<T> mat) => mat.computeDecomposition());
            computationCache.AddCacheEntry("identity", false, true, (Matrix<T> mat) => mat.testForIdentity());
            computationCache.AddCacheEntry("cofactorMatrix", null, true, (Matrix<T> mat) => mat.computeCofactorMatrix());
            computationCache.AddCacheEntry("inverse", null, true, (Matrix<T> mat) => mat.computeInverse());
            computationCache.AddCacheEntry("rank", 0, true, (Matrix<T> mat) => mat.computeRank());
        }

        #endregion

        public Matrix<T> SubMatrix(int row, int col)
        {
            int rowIndex = 0;
            int colIndex = 0;

            T[,] subMatrix = new T[Height - (row < 0 ? 0 : 1), Width - (col < 0 ? 0 : 1)];

            for (int i = 0; i < Height; i++)
            {
                if (i != row)
                {
                    colIndex = 0;

                    for (int j = 0; j < Width; j++)
                    {
                        if (j != col)
                        {
                            subMatrix[rowIndex, colIndex] = this[i, j];

                            colIndex++;
                        }
                    }

                    rowIndex++;
                }
            }

            return new Matrix<T>(subMatrix);
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
                    temp[temp.Height - 1, i] = row[i].Clone();
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
                    temp[index, i] = row[i].Clone();
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
                    temp[i, temp.Columns - 1] = col[i].Clone();
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
                    temp[i, index] = col[i].Clone();
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
            return SubMatrix(row, col).Determinant;
        }

        public T Cofactor(int row, int col)
        {
            return Minor(row, col).Multiply((row + col) % 2 == 0 ? fieldOne : fieldOne.AdditiveInverse);
        }

        private Matrix<T> computeCofactorMatrix()
        {
            if (!IsSquareMatrix)
            {
                throw new InvalidOperationException("A non-square matrix doesn't have a cofactor matrix.");
            }

            Matrix<T> cofactorMat = new Matrix<T>(Height, Width);

            for (int i = 0; i < cofactorMat.Height; i++)
            {
                for (int j = 0; j < cofactorMat.Width; j++)
                {
                    cofactorMat[i, j] = Cofactor(i, j);
                }
            }

            return cofactorMat;
        }

        private LUPDecomposition<T> computeDecomposition()
        {
            return new LUPDecomposition<T>(this);
        }

        private T computeDeterminant()
        {
            if (!IsSquareMatrix)
            {
                throw new InvalidOperationException("A non-square matrix doesn't have a determinant.");
            }

            if (Height == 1)
            {
                return this[0, 0];
            }

            T det = Decomposition.LowerMatrix.MainDiagonal.FieldProduct().Multiply(Decomposition.UpperMatrix.MainDiagonal.FieldProduct());

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
            Matrix<T> reducedMat = new Matrix<T>(this);
            List<MatrixOperation<T>> reductionOperations = new List<MatrixOperation<T>>();

            bool isZeroColumn = false;

            int rowOffset = 0;

            for (int col = 0; col < Math.Min(reducedMat.Rows, reducedMat.Columns); col++)
            {
                isZeroColumn = false;
                if (reducedMat[col - rowOffset, col].Equals(fieldZero))
                {
                    isZeroColumn = true;
                    for (int row = col + 1 - rowOffset; row < reducedMat.Height; row++)
                    {
                        if (!reducedMat[row, col].Equals(fieldZero))
                        {
                            reductionOperations.Add(new SwapRows<T>(row, col - rowOffset));
                            reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedMat);
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

                reductionOperations.Add(new MultiplyRowByScalar<T>(col - rowOffset, reducedMat[col - rowOffset, col].MultiplicativeInverse));
                reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedMat);

                reducedMat[col - rowOffset, col] = fieldOne.Clone(); // This line is in place to eliminate the possibility of rounding errors

                for (int row = col + 1 - rowOffset; row < reducedMat.Rows; row++)
                {
                    reductionOperations.Add(new AddRowMultiple<T>(col - rowOffset, row, reducedMat[row, col].AdditiveInverse));
                    reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedMat);

                    reducedMat[row, col] = fieldZero.Clone(); // This line is in place to eliminate the possibility of rounding errors
                }
            }

            return new Tuple<Matrix<T>, List<MatrixOperation<T>>>(reducedMat, reductionOperations);
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

                    reducedEchelonFormMatrix[currentRow, entryIndex] = fieldZero.Clone(); // This line is in place to eliminate the possibility of rounding errors
                }
            }

            List<MatrixOperation<T>> reducedEchelonFormOperations = new List<MatrixOperation<T>>();
            reducedEchelonFormOperations.AddRange(EchelonFormReductionOperations);
            reducedEchelonFormOperations.AddRange(reductionOperations);

            return new Tuple<Matrix<T>, List<MatrixOperation<T>>>(reducedEchelonFormMatrix, reducedEchelonFormOperations);
        }

        private Matrix<T> computeInverse()
        {
            if (!IsSquareMatrix)
            {
                throw new InvalidOperationException("A non-square matrix doesn't have an inverse.");
            }
            else if (Determinant.Equals(fieldZero))
            {
                throw new InvalidOperationException("A matrix with determinant 0 doesn't have an inverse.");
            }

            Matrix<T> inverseMat = new Matrix<T>(Rows, Columns);

            for (int i = 0; i < Rows; i++)
            {
                inverseMat[i, i] = new T().One;
            }

            foreach (var op in ReducedEchelonFormReductionOperations)
            {
                op.ApplyTo(inverseMat);
            }

            return inverseMat;
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

        private int computeRank()
        {
            Matrix<T> reducedMat = ReducedEchelonForm;
            int rank = Rows;

            for (int i = 0; i < Height; i++)
            {
                bool isZeroRow = true;

                for (int j = 0; j < Width; j++)
                {
                    if (this[i, j] != fieldZero)
                    {
                        isZeroRow = false;
                        break;
                    }
                }

                if (isZeroRow)
                {
                    rank--;
                }
            }

            return rank;
        }

        #region Operators
        public static Matrix<T> operator +(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Width || left.Height != right.Height)
            {
                throw new InvalidOperationException("Matrices of different sizes can't be added.");
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
                throw new InvalidOperationException("Matrices of different sizes can't be subtracted.");
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
                throw new InvalidOperationException("Matrices of incompatible sizes can't be multiplied.");
            }

            T[,] multResult = new T[left.Height, right.Width];

            for (int i = 0; i < multResult.GetLength(0); i++)
            {
                for (int j = 0; j < multResult.GetLength(1); j++)
                {
                    multResult[i, j] = new T().Zero;

                    for (int k = 0; k < left.Width; k++)
                    {
                        multResult[i, j] += left.definingArray[i, k] * right.definingArray[k, j];
                    }
                }
            }

            return new Matrix<T>(multResult);
        }

        /*
         * Only effective on matrices that are at least 6x6
         */
        public static Matrix<T> ParallelMultiply(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("Matrices of incompatible sizes can't be multiplied.");
            }

            T[,] temp = new T[left.Height, right.Width];

            Parallel.For(0, left.Height * right.Width, (index) =>
            {
                int rowIndex = index / left.Height;
                int colIndex = index % right.Width;

                temp[rowIndex, colIndex] = new T().Zero;

                for (int k = 0; k < left.Width; k++)
                {
                    temp[rowIndex, colIndex] += left.definingArray[rowIndex, k] * right.definingArray[k, colIndex];
                }
            });

            return new Matrix<T>(temp);
        }

        private static void MulKernel<U>(U[,] leftMat, U[,] rightMat, U[,] resultMat, Func<U, U, U> addOp, Func<U, U, U> mulOp)
        {
            int leftHeight = leftMat.GetLength(0);
            int leftWidth = leftMat.GetLength(1);

            int rightWidth = rightMat.GetLength(0);

            int index = blockDim.x * blockIdx.x + threadIdx.x;
            int rowIndex = index / leftHeight;
            int colIndex = index % rightWidth;

            if (rowIndex >= resultMat.GetLength(0) || colIndex >= resultMat.GetLength(1))
            {
                return;
            }

            for (int k = 0; k < leftWidth; k++)
            {
                /*var lv = leftMat[rowIndex, k];
                var rv = rightMat[k, colIndex];
                var mul = mulOp(lv, rv);
                var tv = resultMat[rowIndex, colIndex];
                var sum = addOp(tv, mul);

                resultMat[rowIndex, colIndex] = sum;*/
                resultMat[rowIndex, colIndex] = addOp(resultMat[rowIndex, colIndex], mulOp(leftMat[rowIndex, k], rightMat[k, colIndex]));
            }
        }

        public static Matrix<FieldType> GpuMultiply<FieldType, GpuStructType>(Matrix<FieldType> left, Matrix<FieldType> right) where FieldType : Field<FieldType>, IGpuCompatible<FieldType, GpuStructType>, new() where GpuStructType : struct
        {
            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("Matrices of incompatible sizes can't be multiplied.");
            }

            IGpuStructManager<FieldType, GpuStructType> gpuStructManager = new FieldType().GetDefaultGpuStructManager();

            GpuStructType[,] resultArr = new GpuStructType[left.Rows, right.Columns];
            GpuStructType[,] leftArr = new GpuStructType[left.Rows, left.Columns];
            GpuStructType[,] rightArr = new GpuStructType[right.Rows, right.Columns];

            resultArr.Initialize(gpuStructManager.GetStructDefaultValue());
            leftArr.Initialize((ind) => gpuStructManager.ToStruct(left.definingArray[ind[0], ind[1]]));
            rightArr.Initialize((ind) => gpuStructManager.ToStruct(right.definingArray[ind[0], ind[1]]));

            
            Gpu gpu = Gpu.Default;

            int threadCount = left.Rows * right.Columns;
            int blockDimX = gpu.Device.Attributes.MaxThreadsPerBlock; // Threads per block
            int gridDimX = (int)Math.Ceiling((double)threadCount / blockDimX); // Blocks per thread

            LaunchParam lp = new LaunchParam(gridDimX, blockDimX);
            gpu.Launch(MulKernel, lp, leftArr, rightArr, resultArr, gpuStructManager.GetStructAddition(), gpuStructManager.GetStructMultiplication());
            
            /*var addOp = gpuStructManager.GetStructAddition();
            var mulOp = gpuStructManager.GetStructMultiplication();
            Gpu.Default.For(0, resultArr.GetLength(0) * resultArr.GetLength(1), (index) =>
            {
                int leftHeight = leftArr.GetLength(0);
                int leftWidth = leftArr.GetLength(1);

                int rightWidth = rightArr.GetLength(0);

                int rowIndex = index / leftHeight;
                int colIndex = index % rightWidth;

                for (int k = 0; k < leftWidth; k++)
                {
                    var lv = leftArr[rowIndex, k];
                    var rv = rightArr[k, colIndex];
                    var mul = mulOp(lv, rv);
                    var tv = resultArr[rowIndex, colIndex];
                    var sum = addOp(tv, mul);

                    resultArr[rowIndex, colIndex] = sum;
                    //resultMat[rowIndex, colIndex] = addOp(resultMat[rowIndex, colIndex], mulOp(leftMat[rowIndex, k], rightMat[k, colIndex]));
                }
            });*/

            FieldType[,] fieldResultArr = new FieldType[resultArr.GetLength(0), resultArr.GetLength(1)];
            fieldResultArr.Initialize((ind) => gpuStructManager.ToClass(resultArr[ind[0], ind[1]]));

            return new Matrix<FieldType>(fieldResultArr);
        }

        public static implicit operator T[,] (Matrix<T> mat)
        {
            T[,] defArrClone = new T[mat.Rows, mat.Columns];

            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Columns; j++)
                {
                    defArrClone[i, j] = mat[i, j].Clone();
                }
            }

            return defArrClone;
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