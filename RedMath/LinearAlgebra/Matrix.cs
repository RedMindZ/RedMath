using Alea;
using Alea.CSharp;
using Alea.Parallel;
using RedMath.LinearAlgebra.MatrixOperations;
using RedMath.ParallelComputation.GpuUtils;
using RedMath.Structures;
using RedMath.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.LinearAlgebra
{
    public class Matrix<T> where T : Field<T>, new()
    {
        #region Class Fields
        protected T[,] definingArray;
        protected Cache<string> computationCache = new Cache<string>();

        private static T fieldZero = new T().Zero;
        private static T fieldOne = new T().One;
        #endregion

        #region Size Properties
        public int Height => definingArray.GetLength(0);
        public int Width => definingArray.GetLength(1);

        public int Rows => definingArray.GetLength(0);
        public int Columns => definingArray.GetLength(1);
        #endregion

        #region Matrix Properties
        public bool IsRowMatrix => Rows == 1;
        public bool IsColumnMatrix => Columns == 1;

        public bool IsSquareMatrix => Rows == Columns;

        public bool IsIdentity => computationCache.RetrieveValue<bool, Matrix<T>>("identity", this);

        public bool IsLowerTriangular => computationCache.RetrieveValue<bool, Matrix<T>>("lowerTriangular", this);
        public bool IsUpperTriangular => computationCache.RetrieveValue<bool, Matrix<T>>("upperTriangular", this);
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

        public LUPDecomposition<T> Decomposition => new LUPDecomposition<T>(computationCache.RetrieveValue<LUPDecomposition<T>, Matrix<T>>("decomposition", this));

        public T Determinant => computationCache.RetrieveValue<T, Matrix<T>>("determinant", this);

        public List<IMatrixOperation<T>> EchelonFormReductionOperations => new List<IMatrixOperation<T>>(computationCache.RetrieveValue<List<IMatrixOperation<T>>, Matrix<T>>("echelonFormOperations", this));
        public List<IMatrixOperation<T>> ReducedEchelonFormReductionOperations => new List<IMatrixOperation<T>>(computationCache.RetrieveValue<List<IMatrixOperation<T>>, Matrix<T>>("reducedEchelonFormOperations", this));

        public Matrix<T> EchelonForm => new Matrix<T>(computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("echelonForm", this));
        public Matrix<T> ReducedEchelonForm => new Matrix<T>(computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("reducedEchelonForm", this));

        public Matrix<T> CofactorMatrix => new Matrix<T>(computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("cofactorMatrix", this));

        public Matrix<T> Inverse => computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("inverse", this);

        public int Rank => computationCache.RetrieveValue<int, Matrix<T>>("rank", this);

        public bool IsFullRank => IsSquareMatrix && Rank == Rows;

        public Matrix<T> Transposition
        {
            get
            {
                Matrix<T> mat = new Matrix<T>(this);
                mat.Transpose();

                return mat;
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

            definingArray.Assign(ind => fieldZero.Clone());

            initCache();
        }

        public Matrix(T[,] data)
        {
            definingArray = new T[data.GetLength(0), data.GetLength(1)];

            definingArray.Assign(ind => data[ind[0], ind[1]].Clone());

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
            computationCache.AddCacheEntry("lowerTriangular", false, true, (Matrix<T> mat) => mat.testForLowerTriangular());
            computationCache.AddCacheEntry("upperTriangular", false, true, (Matrix<T> mat) => mat.testForUpperTriangular());
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
                            subMatrix[rowIndex, colIndex] = definingArray[i, j];

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
            T[,] temp = new T[Columns, Rows];

            temp.Assign(ind => definingArray[ind[1], ind[0]]);

            definingArray = temp;

            computationCache.SetAllUpdateStates(true);
        }

        public Vector<T> GetRowVector(int index)
        {
            T[] res = new T[Width];

            res.Assign(ind => definingArray[index, ind[0]].Clone());

            return new Vector<T>(res);
        }

        public Vector<T> GetColumnVector(int index)
        {
            T[] res = new T[Height];

            res.Assign(ind => definingArray[ind[0], index].Clone());

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

            for (int i = 0; i < temp.Columns; i++)
            {
                if (i < row.Dimension)
                {
                    temp[temp.Rows - 1, i] = row[i].Clone();
                }
                else
                {
                    temp[temp.Rows - 1, i] = fieldZero.Clone();
                }
            }

            definingArray = temp.definingArray;

            computationCache.SetAllUpdateStates(true);
        }

        public void InsertRowVector(Vector<T> row, int index)
        {
            Matrix<T> temp = new Matrix<T>(Rows + 1, Columns);

            for (int i = 0, rowIndex = 0; i < temp.Rows; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Columns; j++)
                {
                    temp[i, j] = definingArray[rowIndex, j];
                }

                rowIndex++;
            }

            if (row.Dimension > temp.Width)
            {
                temp.Resize(temp.Rows, row.Dimension);
            }

            for (int i = 0; i < temp.Columns; i++)
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

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    temp[i, j] = definingArray[i, j];
                }
            }

            if (col.Dimension > temp.Height)
            {
                temp.Resize(col.Dimension, temp.Columns);
            }

            for (int i = 0; i < temp.Rows; i++)
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
            
            for (int i = 0, colIndex = 0; i < temp.Columns; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Rows; j++)
                {
                    temp[j, i] = definingArray[j, colIndex];
                }

                colIndex++;
            }

            if (col.Dimension > temp.Height)
            {
                temp.Resize(col.Dimension, temp.Columns);
            }

            for (int i = 0; i < temp.Rows; i++)
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

            T[,] cofactorMat = new T[Rows, Columns];

            cofactorMat.Assign(ind => Cofactor(ind[0], ind[1]));

            return new Matrix<T>(cofactorMat);
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
                return definingArray[0, 0];
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

        private Tuple<Matrix<T>, List<IMatrixOperation<T>>> computeEchelonForm()
        {
            Matrix<T> reducedMat = new Matrix<T>(this);
            List<IMatrixOperation<T>> reductionOperations = new List<IMatrixOperation<T>>();

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

            return new Tuple<Matrix<T>, List<IMatrixOperation<T>>>(reducedMat, reductionOperations);
        }

        private Tuple<Matrix<T>, List<IMatrixOperation<T>>> computeReducedEchelonForm()
        {
            Matrix<T> reducedEchelonFormMatrix = EchelonForm;
            List<IMatrixOperation<T>> reductionOperations = new List<IMatrixOperation<T>>();

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

            List<IMatrixOperation<T>> reducedEchelonFormOperations = new List<IMatrixOperation<T>>();
            reducedEchelonFormOperations.AddRange(EchelonFormReductionOperations);
            reducedEchelonFormOperations.AddRange(reductionOperations);

            return new Tuple<Matrix<T>, List<IMatrixOperation<T>>>(reducedEchelonFormMatrix, reducedEchelonFormOperations);
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

        private bool testForLowerTriangular()
        {
            if (!IsSquareMatrix)
            {
                return false;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = row + 1; col < Columns; col++)
                {
                    if (definingArray[row, col] != fieldZero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool testForUpperTriangular()
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

        private int computeRank()
        {
            Matrix<T> reducedMat = ReducedEchelonForm;
            int rank = Rows;

            for (int i = 0; i < Height; i++)
            {
                bool isZeroRow = true;

                for (int j = 0; j < Width; j++)
                {
                    if (definingArray[i, j] != fieldZero)
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

            temp.Assign(ind => left[ind[0], ind[1]] + right[ind[0], ind[1]]);
            
            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Width || left.Height != right.Height)
            {
                throw new InvalidOperationException("Matrices of different sizes can't be subtracted.");
            }

            T[,] temp = new T[left.Height, left.Width];

            temp.Assign(ind => left[ind[0], ind[1]] - right[ind[0], ind[1]]);

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> mat)
        {
            T[,] temp = new T[mat.Height, mat.Width];

            temp.Assign(ind => -mat[ind[0], ind[1]]);

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(Matrix<T> mat, T scalar)
        {
            T[,] temp = new T[mat.Height, mat.Width];

            temp.Assign(ind => mat[ind[0], ind[1]] * scalar);

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(T scalar, Matrix<T> mat)
        {
            return mat * scalar;
        }

        public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("Matrices of incompatible sizes can't be multiplied.");
            }

            T[,] multResult = new T[left.Height, right.Width];

            multResult.Assign(ind => fieldZero.Clone());

            multResult.Assign(ind =>
            {
                int i = ind[0];
                int j = ind[1];

                T sum = fieldZero.Clone();

                for (int k = 0; k < left.Width; k++)
                {
                    sum += left.definingArray[i, k] * right.definingArray[k, j];
                }

                return sum;
            });

            return new Matrix<T>(multResult);
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
                    if (definingArray[i, j] != other[i, j])
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
                    sum += definingArray[i, j].GetHashCode();
                }
            }

            return sum;
        }

        public override string ToString()
        {
            int digits = 0;
            int[] charCount = new int[Columns];

            StringBuilder res = new StringBuilder();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    digits = definingArray[i, j].ToString().Length;

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
                    res.Append(definingArray[i, j]);

                    if (j < Columns - 1)
                    {
                        digits = definingArray[i, j].ToString().Length;

                        for (int k = digits; k < charCount[j]; k++)
                        {
                            res.Append(' ');
                        }
                    }
                }

                if (i < Rows - 1)
                {
                    res.Append('\n');
                }
            }

            return res.ToString();
        }
        #endregion
    }
}