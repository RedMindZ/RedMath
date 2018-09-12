using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RedMath.Structures;
using RedMath.LinearAlgebra.MatrixOperations;

namespace RedMath.LinearAlgebra
{

    public class VectorSpace<T> where T : Field<T>, new()
    {
        protected Vector<T>[] basis;
        protected Matrix<T> basisMatrix;

        public Vector<T>[] Basis
        {
            get
            {
                Vector<T>[] basisCopy = new Vector<T>[basis.Length];

                for (int i = 0; i < basis.Length; i++)
                {
                    basisCopy[i] = new Vector<T>(basis[i]);
                }

                return basisCopy;
            }
        }

        public int Dimension { get; private set; }

        public VectorSpace(params Vector<T>[] vectorsBasisCollection)
        {
            basis = new Vector<T>[vectorsBasisCollection.Length];

            for (int i = 0; i < basis.Length; i++)
            {
                basis[i] = new Vector<T>(vectorsBasisCollection[i]);
            }


            for (int i = 0; i < basis.Length; i++)
            {
                if (basis[i].Dimension != basis[0].Dimension)
                {
                    throw new ArgumentException("All vectors that are passed to the VectorSpace's constructor must be of the same dimension.");
                }
            }

            Dimension = basis[0].Dimension;

            /*if (basis[0].Dimension != basis.Length)
            {
                throw new ArgumentException("The number of vectors passed to the VectorSpace's constructor must be equal to the dimension of each vector.");
            }*/

            basisMatrix = new Matrix<T>();

            for (int i = 0; i < basis.Length; i++)
            {
                basisMatrix.AppendColumnVector(basis[i]);
            }

            if (basisMatrix.Rank < basisMatrix.Columns)
            {
                throw new ArgumentException("The vectors passed to the VectorSpace's constructor must be linearly independent.");
            }
        }

        public Matrix<T> ChangeToThisBasisMatrix(VectorSpace<T> other)
        {
            Matrix<T> fromBasisMatrix = new Matrix<T>();
            Matrix<T> toBasisMatrix = new Matrix<T>();

            if (other.basis.Length != basis.Length)
            {
                throw new ArgumentException("The two vector spaces must be of the same dimension.");
            }

            for (int i = 0; i < basis.Length; i++)
            {
                if (!IsVectorInSpace(other.basis[i]) || !other.IsVectorInSpace(basis[i]))
                {
                    throw new ArgumentException("A change of basis matrix can only be created for two equivalent vector spaces.");
                }
            }

            foreach (Vector<T> vec in other.basis)
            {
                fromBasisMatrix.AppendColumnVector(vec);
            }

            foreach (IMatrixOperation<T> op in toBasisMatrix.ReducedEchelonFormReductionOperations)
            {
                op.ApplyTo(fromBasisMatrix);
            }

            return fromBasisMatrix;
        }

        public Matrix<T> ChangeFromThisBasisMatrix(VectorSpace<T> other)
        {
            return other.ChangeToThisBasisMatrix(this);
        }

        public bool IsVectorInSpace(Vector<T> vec)
        {
            if (vec.Dimension != basis[0].Dimension)
            {
                return false;
            }

            Matrix<T> reducedVec = vec.ToColumnMatrix();
            foreach (IMatrixOperation<T> op in basisMatrix.ReducedEchelonFormReductionOperations)
            {
                op.ApplyTo(reducedVec);
            }

            for (int i = basis.Length; i < Dimension; i++)
            {
                if (reducedVec[i, 0] != Field<T>.Zero)
                {
                    return false;
                }
            }

            return true;
        }

        public Vector<T> GetVectorCoordinates(Vector<T> vec)
        {
            if (!IsVectorInSpace(vec))
            {
                throw new ArgumentException("The vector passed to the function is not in the vector space and its coordinates can not be calculated.");
            }

            Matrix<T> coordinatesVector = vec.ToColumnMatrix();
            foreach (IMatrixOperation<T> op in basisMatrix.ReducedEchelonFormReductionOperations)
            {
                op.ApplyTo(coordinatesVector);
            }

            coordinatesVector.Resize(basis.Length, 1);

            return coordinatesVector.GetColumnVector(0);
        }
    }
}
