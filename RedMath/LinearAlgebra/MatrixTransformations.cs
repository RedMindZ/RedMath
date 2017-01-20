using RedMath.Structures;
using System;

namespace RedMath.LinearAlgebra
{
    public static class MatrixTransformations
    {
        public static Matrix<Real> Rotation(Matrix<Real> axis, double angle, bool degrees = false)
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

            double[] vec = new double[n];

            for (int i = 0; i < n; i++)
            {
                vec[i] = 1;
            }

            for (int i = 0; i < n; i++)
            {
                A_t[i, i] = 0;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    A_t[i, j] = axis.Submatrix(i, -1).Cofactor(j, -1);
                    A_t[j, i] = -A_t[i, j];
                }
            }

            return I + A_t * A_t * (1 - Math.Cos(angle)) + A_t * Math.Sin(angle);
        }
    }
}
