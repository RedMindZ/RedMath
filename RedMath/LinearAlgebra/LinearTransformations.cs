using RedMath.Structures;
using System;

namespace RedMath.LinearAlgebra
{
    public static class LinearTransformations
    {
        public static Matrix<Real> CreateRotationMatrix(Matrix<Real> axis, double angle, bool degrees = false)
        {
            if (degrees)
            {
                angle = angle * Math.PI / 180;
            }

            if (axis == null)
            {
                return new Matrix<Real>
                    (
                        new Real[,]
                        {
                            { Math.Cos(angle), -Math.Sin(angle) },
                            { Math.Sin(angle),  Math.Cos(angle) }
                        }
                    );
            }

            int n = axis.Height;
            Matrix<Real> I = new Matrix<Real>(n, n);
            Matrix<Real> A_t = new Matrix<Real>(n, n);

            for (int i = 0; i < n; i++)
            {
                A_t[i, i] = 0;
                I[i, i] = 1;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    A_t[i, j] = axis.SubMatrix(i, -1).Cofactor(j, -1);
                    A_t[j, i] = -A_t[i, j];
                }
            }

            return I + A_t * A_t * (1 - Math.Cos(angle)) + A_t * Math.Sin(angle);
        }

        public static Matrix<T> CreateTranslationMatrix<T>(Vector<T> vec) where T : Field<T>, new()
        {
            Matrix<T> mat = new Matrix<T>(vec.Dimension, vec.Dimension + 1);

            for (int i = 0; i < mat.Height; i++)
            {
                mat[i, i] = new T().One;
            }

            mat.AppendRowVector(vec.HomogeneousCoordinates);

            return mat;
        }

        public static Matrix<Real> CreateLookAtMatrix(Vector<Real> from, Vector<Real> to, params Vector<Real>[] directions)
        {
            if (from.Dimension < 2 || to.Dimension < 2)
            {
                throw new ArgumentException("The dimension of the 'form' and 'to' vectors should be atleast 2", "from, to");
            }

            if (from.Dimension != to.Dimension)
            {
                throw new ArgumentException("The dimension of the 'form' and 'to' vectors should equall", "from, to");
            }

            if (directions.Length != from.Dimension - 2)
            {
                throw new ArgumentException("There should be n-2 direction vectors, where n is the dimension of the 'form' and 'to' vectors", "directions");
            }

            for (int i = 0; i < directions.Length; i++)
            {
                if (from.Dimension != directions[i].Dimension)
                {
                    throw new ArgumentException("All vectors should be the of the same dimension");
                }
            }

            Matrix<Real> mat = new Matrix<Real>();
            Vector<Real>[] vectorList = new Vector<Real>[from.Dimension - 1];

            mat.AppendColumnVector((to - from).GetNormalizedVector());

            for (int i = 0; i < from.Dimension - 1; i++)
            {
                for (int j = 0; j < directions.Length - i; j++)
                {
                    vectorList[j] = directions[j];
                }

                for (int j = directions.Length - i; j < from.Dimension - 1; j++)
                {
                    vectorList[j] = mat.GetColumnVector(j - (directions.Length - i));
                }
                //Vector<Real> temp = Vector<Real>.CrossProduct(vectorList);
                mat.InsertColumnVector(Vector<Real>.CrossProduct(vectorList).GetNormalizedVector(), 0);
            }

            mat.Resize(mat.Rows + 1, mat.Columns + 1);

            //mat.AppendColumnVector(new Vector<Real>(from.Dimension));
            //mat.AppendRowVector(new Vector<Real>(from.Dimension));

            mat[mat.Rows - 1, mat.Columns - 1] = 1;

            return mat;
        }

        public static Matrix<Real> CreatePerspectiveProjectionMatrix(Real eyeAngle, int dimension)
        {
            if (dimension < 3)
            {
                throw new ArgumentException("The dimension should be 3 or higher", "dimension");
            }

            Matrix<Real> mat = new Matrix<Real>(dimension + 1, dimension + 1);

            Real val = 1 / Math.Tan(eyeAngle / 2);

            for (int i = 0; i < dimension - 1; i++)
            {
                mat[i, i] = val;
            }

            return mat;
        }

        public static Matrix<Real> CreateFullPerspectiveProjectionMatrix(Real eyeAngle, Real aspect, Real near, Real far, int dimension)
        {
            if (dimension < 3)
            {
                throw new ArgumentException("The dimension should be 3 or higher", "dimension");
            }

            Matrix<Real> mat = new Matrix<Real>(dimension + 1, dimension + 1);

            Real val = 1 / Math.Tan(eyeAngle / 2);

            for (int i = 0; i < dimension - 1; i++)
            {
                mat[i, i] = val;
            }

            mat[0, 0] /= aspect;

            mat[dimension - 1, dimension - 1] = (near + far) / (near - far);
            mat[dimension, dimension - 1] = -2 * ((near * far) / (near - far));
            mat[dimension - 1, dimension] = -1;
            mat[dimension, dimension] = 0;

            return mat;
        }

        public static Matrix<Real> CreatePerspectiveViewMatrix(Real eyeAngle, Vector<Real> from, Vector<Real> to, params Vector<Real>[] directions)
        {
            /*Matrix<Real> translation = CreateTranslationMatrix(-from);
            Matrix<Real> lookAt = CreateLookAtMatrix(from, to, directions);
            Matrix<Real> projMat = CreatePerspectiveProjectionMatrix(eyeAngle, from.Dimension);*/

            //return translation * lookAt * projMat;
            return CreateTranslationMatrix(-from) * CreateLookAtMatrix(from, to, directions) * CreatePerspectiveProjectionMatrix(eyeAngle, from.Dimension);
        }

        public static Matrix<Real> CreateFullPerspectiveViewMatrix(Real eyeAngle, Real aspect, Real near, Real far, Vector<Real> from, Vector<Real> to, params Vector<Real>[] directions)
        {
            /*Matrix<Real> translation = CreateTranslationMatrix(-from);
            Matrix<Real> lookAt = CreateLookAtMatrix(from, to, directions);
            Matrix<Real> projMat = CreateFullPerspectiveProjectionMatrix(eyeAngle, aspect, near, far, from.Dimension);
            
            return translation * lookAt * projMat;*/
            return CreateTranslationMatrix(-from) * CreateLookAtMatrix(from, to, directions) * CreateFullPerspectiveProjectionMatrix(eyeAngle, aspect, near, far, from.Dimension);
        }

        public static Vector<Real> PerspectivePorjection(Vector<Real> vert, Real eyeAngle, Vector<Real> from, Vector<Real> to, params Vector<Real>[] directions)
        {
            Vector<Real> proj = (CreatePerspectiveViewMatrix(eyeAngle, from, to, directions) * vert.ToRowMatrix()).GetRowVector(0);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < proj.Dimension - 1; j++)
                {
                    proj[j] = proj[j] / proj.Last;
                }

                proj.Dimension--;
            }

            return proj;
        }

        public static Vector<Real> FullPerspectivePorjection(Vector<Real> vert, Real eyeAngle, Real aspect, Real near, Real far, Vector<Real> from, Vector<Real> to, params Vector<Real>[] directions)
        {
            Vector<Real> proj = (vert.HomogeneousCoordinates.ToRowMatrix() * CreateFullPerspectiveViewMatrix(eyeAngle, aspect, near, far, from, to, directions)).GetRowVector(0);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < proj.Dimension - 1; j++)
                {
                    proj[j] = proj[j] / proj.Last;
                }

                proj.Dimension--;
            }

            return proj;
        }
    }
}
