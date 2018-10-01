using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using RedMath.LinearAlgebra.MatrixOperations;
using RedMath.Structures;
using RedMath.Utils;

namespace RedMath.LinearAlgebra
{
    public sealed class Matrix<T> where T : Field<T>, new()
    {
        #region Class Fields
        private T[,] _elements;
        private Cache<string> _computationCache = new Cache<string>();

        //private static T _fieldZero = new T()._zero;
        //private static T _fieldOne = new T()._one;
        #endregion

        #region Size Properties
        public int Height => _elements.GetLength(0);
        public int Width => _elements.GetLength(1);

        public int Rows => _elements.GetLength(0);
        public int Columns => _elements.GetLength(1);
        #endregion

        #region Matrix Properties
        public bool IsRowMatrix => Rows == 1;
        public bool IsColumnMatrix => Columns == 1;

        public bool IsSquareMatrix => Rows == Columns;

        public bool IsIdentity => _computationCache.RetrieveValue<bool, Matrix<T>>("identity", this);

        public bool IsLowerTriangular => _computationCache.RetrieveValue<bool, Matrix<T>>("lowerTriangular", this);
        public bool IsUpperTriangular => _computationCache.RetrieveValue<bool, Matrix<T>>("upperTriangular", this);
        #endregion

        #region Diagonals
        public T[] MainDiagonal
        {
            get
            {
                T[] seq = new T[Math.Min(Height, Width)];

                seq.AssignAll(ind => _elements[ind[0], ind[0]].Clone());

                return seq;
            }
        }

        public T[] AntiDiagonal
        {
            get
            {
                T[] seq = new T[Math.Min(Height, Width)];

                seq.AssignAll(ind => _elements[seq.Length - ind[0] - 1, ind[0]].Clone());

                return seq;
            }
        }
        #endregion

        public LUPDecomposition<T> Decomposition => new LUPDecomposition<T>(_computationCache.RetrieveValue<LUPDecomposition<T>, Matrix<T>>("decomposition", this));

        public T Determinant => _computationCache.RetrieveValue<T, Matrix<T>>("determinant", this);

        public List<IMatrixOperation<T>> EchelonFormReductionOperations => new List<IMatrixOperation<T>>(_computationCache.RetrieveValue<List<IMatrixOperation<T>>, Matrix<T>>("echelonFormOperations", this));
        public List<IMatrixOperation<T>> ReducedEchelonFormReductionOperations => new List<IMatrixOperation<T>>(_computationCache.RetrieveValue<List<IMatrixOperation<T>>, Matrix<T>>("reducedEchelonFormOperations", this));

        public Matrix<T> EchelonForm => new Matrix<T>(_computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("echelonForm", this));
        public Matrix<T> ReducedEchelonForm => new Matrix<T>(_computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("reducedEchelonForm", this));

        public Matrix<T> CofactorMatrix => new Matrix<T>(_computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("cofactorMatrix", this));

        public Matrix<T> Inverse => _computationCache.RetrieveValue<Matrix<T>, Matrix<T>>("inverse", this);

        public int Rank => _computationCache.RetrieveValue<int, Matrix<T>>("rank", this);

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
                return _elements[row, col].Clone();
            }

            set
            {
                _elements[row, col] = value.Clone();

                _computationCache.SetAllUpdateStates(true);
            }
        }

        #region Constructors
        public Matrix() : this(0, 0) { }

        public Matrix(int rows, int cols)
        {
            _elements = new T[rows, cols];

            _elements.AssignAll(ind => Field<T>.Zero);

            InitCache();
        }

        public Matrix(T[,] entries)
        {
            _elements = new T[entries.GetLength(0), entries.GetLength(1)];

            _elements.AssignAll(ind => entries[ind[0], ind[1]].Clone());

            InitCache();
        }

        public Matrix(Matrix<T> mat) : this(mat._elements) { }

        private void InitCache()
        {
            _computationCache.AddCacheEntry("determinant", null, true, (Matrix<T> mat) => mat.ComputeDeterminant());
            _computationCache.AddCacheEntry("echelonForm", null, true, (Matrix<T> mat) => mat.ComputeEchelonForm().Item1);
            _computationCache.AddCacheEntry("reducedEchelonForm", null, true, (Matrix<T> mat) => mat.ComputeReducedEchelonForm().Item1);
            _computationCache.AddCacheEntry("echelonFormOperations", null, true, (Matrix<T> mat) => mat.ComputeEchelonForm().Item2);
            _computationCache.AddCacheEntry("reducedEchelonFormOperations", null, true, (Matrix<T> mat) => mat.ComputeReducedEchelonForm().Item2);
            _computationCache.AddCacheEntry("decomposition", null, true, (Matrix<T> mat) => mat.ComputeDecomposition());
            _computationCache.AddCacheEntry("identity", false, true, (Matrix<T> mat) => mat.TestForIdentity());
            _computationCache.AddCacheEntry("cofactorMatrix", null, true, (Matrix<T> mat) => mat.ComputeCofactorMatrix());
            _computationCache.AddCacheEntry("inverse", null, true, (Matrix<T> mat) => mat.ComputeInverse());
            _computationCache.AddCacheEntry("rank", 0, true, (Matrix<T> mat) => mat.ComputeRank());
            _computationCache.AddCacheEntry("lowerTriangular", false, true, (Matrix<T> mat) => mat.TestForLowerTriangular());
            _computationCache.AddCacheEntry("upperTriangular", false, true, (Matrix<T> mat) => mat.TestForUpperTriangular());
        }

        #endregion

        public Matrix<T> SubMatrix(int row, int col)
        {
            int rowIndex = 0;
            int colIndex = 0;

            T[,] subMatrix = new T[Rows - (row < 0 ? 0 : 1), Columns - (col < 0 ? 0 : 1)];

            for (int i = 0; i < Height; i++)
            {
                if (i != row)
                {
                    colIndex = 0;

                    for (int j = 0; j < Width; j++)
                    {
                        if (j != col)
                        {
                            subMatrix[rowIndex, colIndex] = _elements[i, j];

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

            T[,] buffer = new T[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i < Rows && j < Columns)
                    {
                        buffer[i, j] = _elements[i, j];
                    }
                    else
                    {
                        buffer[i, j] = Field<T>.Zero;
                    }
                }
            }

            _elements = buffer;

            _computationCache.SetAllUpdateStates(true);
        }

        public void Transpose()
        {
            T[,] buffer = new T[Columns, Rows];

            buffer.AssignAll(ind => _elements[ind[1], ind[0]]);

            _elements = buffer;

            _computationCache.SetAllUpdateStates(true);
        }

        public Vector<T> GetRowVector(int index)
        {
            T[] res = new T[Width];

            res.AssignAll(ind => this[index, ind[0]]);

            return new Vector<T>(res);
        }

        public Vector<T> GetColumnVector(int index)
        {
            T[] res = new T[Height];

            res.AssignAll(ind => this[ind[0], index]);

            return new Vector<T>(res);
        }

        public void AppendRowVector(Vector<T> row)
        {
            Matrix<T> buffer = new Matrix<T>(Rows + 1, Columns);

            buffer._elements.Assign(ind => _elements[ind[0], ind[1]], new int[] { Rows, Columns });

            if (row.Dimension > buffer.Width)
            {
                buffer.Resize(buffer.Rows, row.Dimension);
            }

            buffer._elements.Assign(ind => row[ind[1]].Clone(), new int[] { buffer.Rows - 1, 0 }, new int[] { buffer.Rows - 1, row.Dimension });

            _elements = buffer._elements;

            _computationCache.SetAllUpdateStates(true);
        }

        public void InsertRowVector(Vector<T> row, int index)
        {
            Matrix<T> buffer = new Matrix<T>(Rows + 1, Columns);

            for (int i = 0, rowIndex = 0; i < buffer.Rows; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Columns; j++)
                {
                    buffer._elements[i, j] = _elements[rowIndex, j];
                }

                rowIndex++;
            }

            if (row.Dimension > buffer.Width)
            {
                buffer.Resize(buffer.Rows, row.Dimension);
            }

            buffer._elements.Assign(ind => row[ind[1]].Clone(), new int[] { index, 0 }, new int[] { index + 1, row.Dimension });

            _elements = buffer._elements;

            _computationCache.SetAllUpdateStates(true);
        }

        public void AppendColumnVector(Vector<T> col)
        {
            Matrix<T> buffer = new Matrix<T>(Rows, Columns + 1);

            buffer._elements.Assign(ind => _elements[ind[0], ind[1]], new int[] { Rows, Columns });

            if (col.Dimension > buffer.Height)
            {
                buffer.Resize(col.Dimension, buffer.Columns);
            }

            buffer._elements.Assign(ind => col[ind[0]].Clone(), new int[] { 0, buffer.Columns - 1 }, new int[] { col.Dimension, buffer.Columns });

            _elements = buffer._elements;

            _computationCache.SetAllUpdateStates(true);
        }

        public void InsertColumnVector(Vector<T> col, int index)
        {
            Matrix<T> buffer = new Matrix<T>(Rows, Columns + 1);

            for (int i = 0, colIndex = 0; i < buffer.Columns; i++)
            {
                if (i == index)
                {
                    continue;
                }

                for (int j = 0; j < Rows; j++)
                {
                    buffer._elements[j, i] = _elements[j, colIndex];
                }

                colIndex++;
            }

            if (col.Dimension > buffer.Height)
            {
                buffer.Resize(col.Dimension, buffer.Columns);
            }

            buffer._elements.Assign(ind => col[ind[0]].Clone(), new int[] { 0, index }, new int[] { col.Dimension, index + 1 });

            _elements = buffer._elements;

            _computationCache.SetAllUpdateStates(true);
        }

        public void AppendMatrixToTheRight(Matrix<T> mat)
        {
            _elements = ConcatRows(this, mat)._elements;
        }

        public void AppendMatrixToTheLeft(Matrix<T> mat)
        {
            _elements = ConcatRows(mat, this)._elements;
        }

        public static Matrix<T> ConcatRows(params Matrix<T>[] matrices)
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                if (matrices[i].Rows != matrices[0].Rows)
                {
                    throw new ArgumentException("All matrices to be concatenated must have the same number of rows.");
                }
            }

            T[,] matEntries = new T[matrices[0].Rows, matrices.Sum((mat) => mat.Columns)];

            int columnOffset = 0;
            for (int i = 0; i < matrices.Length; i++)
            {
                for (int j = 0; j < matrices[i].Rows; j++)
                {
                    for (int k = 0; k < matrices[i].Columns; k++)
                    {
                        matEntries[j, columnOffset + k] = matrices[i][j, k];
                    }
                }

                columnOffset += matrices[i].Columns;
            }

            return new Matrix<T>(matEntries);
        }

        public T Minor(int row, int col)
        {
            return SubMatrix(row, col).Determinant;
        }

        public T Cofactor(int row, int col)
        {
            return Minor(row, col).Multiply((row + col) % 2 == 0 ? Field<T>.One : Field<T>.One.AdditiveInverse);
        }

        private Matrix<T> ComputeCofactorMatrix()
        {
            if (!IsSquareMatrix)
            {
                throw new InvalidOperationException("A non-square matrix doesn't have a cofactor matrix.");
            }

            T[,] cofactorMat = new T[Rows, Columns];

            cofactorMat.AssignAll(ind => Cofactor(ind[0], ind[1]));

            return new Matrix<T>(cofactorMat);
        }

        private LUPDecomposition<T> ComputeDecomposition()
        {
            return new LUPDecomposition<T>(this);
        }

        private T ComputeDeterminant()
        {
            if (!IsSquareMatrix)
            {
                throw new InvalidOperationException("A non-square matrix doesn't have a determinant.");
            }

            if (Height == 1)
            {
                return _elements[0, 0];
            }

            T det = Decomposition.LowerMatrix.MainDiagonal.Product().Multiply(Decomposition.UpperMatrix.MainDiagonal.Product());

            if (Decomposition.Permutation.Signature == 1)
            {
                return det;
            }
            else
            {
                return det.AdditiveInverse;
            }
        }

        private Tuple<Matrix<T>, List<IMatrixOperation<T>>> ComputeEchelonForm()
        {
            Matrix<T> reducedMat = new Matrix<T>(this);
            List<IMatrixOperation<T>> reductionOperations = new List<IMatrixOperation<T>>();

            bool isZeroColumn = false;

            int rowOffset = 0;

            for (int col = 0; col < Math.Min(reducedMat.Rows, reducedMat.Columns); col++)
            {
                isZeroColumn = false;
                if (reducedMat._elements[col - rowOffset, col] == Field<T>.Zero)
                {
                    isZeroColumn = true;
                    for (int row = col + 1 - rowOffset; row < reducedMat.Height; row++)
                    {
                        if (reducedMat._elements[row, col] != Field<T>.Zero)
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

                reductionOperations.Add(new MultiplyRowByScalar<T>(col - rowOffset, reducedMat._elements[col - rowOffset, col].MultiplicativeInverse));
                reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedMat);

                reducedMat._elements[col - rowOffset, col] = Field<T>.One; // This line is in place to eliminate the possibility of rounding errors

                for (int row = col + 1 - rowOffset; row < reducedMat.Rows; row++)
                {
                    reductionOperations.Add(new AddRowMultiple<T>(col - rowOffset, row, reducedMat._elements[row, col].AdditiveInverse));
                    reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedMat);

                    reducedMat._elements[row, col] = Field<T>.Zero; // This line is in place to eliminate the possibility of rounding errors
                }
            }

            return new Tuple<Matrix<T>, List<IMatrixOperation<T>>>(reducedMat, reductionOperations);
        }

        private Tuple<Matrix<T>, List<IMatrixOperation<T>>> ComputeReducedEchelonForm()
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
                    if (reducedEchelonFormMatrix._elements[baseRow, col] == Field<T>.One)
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
                    reductionOperations.Add(new AddRowMultiple<T>(baseRow, currentRow, reducedEchelonFormMatrix._elements[currentRow, entryIndex].AdditiveInverse));
                    reductionOperations[reductionOperations.Count - 1].ApplyTo(reducedEchelonFormMatrix);

                    reducedEchelonFormMatrix._elements[currentRow, entryIndex] = Field<T>.Zero; // This line is in place to eliminate the possibility of rounding errors
                }
            }

            List<IMatrixOperation<T>> reducedEchelonFormOperations = new List<IMatrixOperation<T>>();
            reducedEchelonFormOperations.AddRange(EchelonFormReductionOperations);
            reducedEchelonFormOperations.AddRange(reductionOperations);

            return new Tuple<Matrix<T>, List<IMatrixOperation<T>>>(reducedEchelonFormMatrix, reducedEchelonFormOperations);
        }

        private Matrix<T> ComputeInverse()
        {
            if (!IsSquareMatrix)
            {
                throw new InvalidOperationException("A non-square matrix doesn't have an inverse.");
            }
            else if (!ReducedEchelonForm.IsIdentity)
            {
                throw new InvalidOperationException("This matrix doesn't have an inverse since its reduced echelon form is not the identity matrix.");
            }

            Matrix<T> inverseMat = new Matrix<T>(Rows, Columns);

            for (int i = 0; i < Rows; i++)
            {
                inverseMat._elements[i, i] = Field<T>.One;
            }

            foreach (var op in ReducedEchelonFormReductionOperations)
            {
                op.ApplyTo(inverseMat);
            }

            return inverseMat;
        }

        private bool TestForIdentity()
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
                    if (col == row && _elements[row, col] != Field<T>.One)
                    {
                        isIdentity = false;
                        break;
                    }
                    else if (col != row && _elements[row, col] != Field<T>.Zero)
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

        private bool TestForLowerTriangular()
        {
            if (!IsSquareMatrix)
            {
                return false;
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = row + 1; col < Columns; col++)
                {
                    if (_elements[row, col] != Field<T>.Zero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool TestForUpperTriangular()
        {
            if (!IsSquareMatrix)
            {
                return false;
            }

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < row; col++)
                {
                    if (_elements[row, col] == Field<T>.Zero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private int ComputeRank()
        {
            int rank = 0;
            Matrix<T> reducedForm = ReducedEchelonForm;

            for (int i = 0; i < Math.Min(reducedForm.Rows, reducedForm.Columns); i++)
            {
                if (reducedForm._elements[i, i] == Field<T>.One)
                {
                    rank++;
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

            T[,] buffer = new T[left.Height, left.Width];

            buffer.AssignAll(ind => left._elements[ind[0], ind[1]] + right._elements[ind[0], ind[1]]);

            return new Matrix<T>(buffer);
        }

        public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Width || left.Height != right.Height)
            {
                throw new InvalidOperationException("Matrices of different sizes can't be subtracted.");
            }

            T[,] buffer = new T[left.Height, left.Width];

            buffer.AssignAll(ind => left._elements[ind[0], ind[1]] - right._elements[ind[0], ind[1]]);

            return new Matrix<T>(buffer);
        }

        public static Matrix<T> operator -(Matrix<T> mat)
        {
            T[,] buffer = new T[mat.Height, mat.Width];

            buffer.AssignAll(ind => -mat._elements[ind[0], ind[1]]);

            return new Matrix<T>(buffer);
        }

        public static Matrix<T> operator *(Matrix<T> mat, T scalar)
        {
            T[,] buffer = new T[mat.Height, mat.Width];

            buffer.AssignAll(ind => mat._elements[ind[0], ind[1]] * scalar);

            return new Matrix<T>(buffer);
        }

        public static Matrix<T> operator *(T scalar, Matrix<T> mat)
        {
            return mat * scalar;
        }

        public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
        {
            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("The matrices given are of incompatible sizes and can't be multiplied.");
            }

            T[,] multResult = new T[left.Height, right.Width];

            multResult.AssignAll(ind =>
            {
                int i = ind[0];
                int j = ind[1];

                T sum = Field<T>.Zero;

                for (int k = 0; k < left.Width; k++)
                {
                    sum += left._elements[i, k] * right._elements[k, j];
                }

                return sum;
            });

            return new Matrix<T>(multResult);
        }

        public static Vector<T> operator *(Matrix<T> mat, Vector<T> vec)
        {
            if (mat.Width != vec.Dimension)
            {
                throw new InvalidOperationException("The matrix and vector given are of incompatible sizes and can't be multiplied.");
            }

            T[] multResult = new T[mat.Height];

            multResult.AssignAll(ind =>
            {
                int i = ind[0];

                T sum = Field<T>.Zero;

                for (int k = 0; k < vec.Dimension; k++)
                {
                    sum += mat._elements[i, k] * vec[k];
                }

                return sum;
            });

            return new Vector<T>(multResult);
        }

        public static Vector<T> operator *(Vector<T> vec, Matrix<T> mat)
        {
            if (vec.Dimension != mat.Height)
            {
                throw new InvalidOperationException("The vector and matrix given are of incompatible sizes and can't be multiplied.");
            }

            T[] multResult = new T[mat.Width];

            multResult.AssignAll(ind =>
            {
                int j = ind[0];

                T sum = Field<T>.Zero;

                for (int k = 0; k < vec.Dimension; k++)
                {
                    sum += vec[k] * mat._elements[k, j];
                }

                return sum;
            });

            return new Vector<T>(multResult);
        }

        public static implicit operator T[,] (Matrix<T> mat)
        {
            T[,] defArrClone = new T[mat.Rows, mat.Columns];

            defArrClone.AssignAll(ind => mat[ind[0], ind[1]]);

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
            if (!(obj is Matrix<T> other))
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
                    if (_elements[i, j] != other._elements[i, j])
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
                    sum += _elements[i, j].GetHashCode();
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
                    digits = _elements[i, j].ToString().Length;

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
                    res.Append(_elements[i, j]);

                    if (j < Columns - 1)
                    {
                        digits = _elements[i, j].ToString().Length;

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