namespace RedMath.LinearAlgebra
{
    public static class MatrixTransformations
    {
        public static Matrix Rotation(Matrix axis, double angle, bool degrees = false)
        {
            if (degrees)
            {
                angle = angle * Algebra.PI / 180;
            }

            angle = -angle;

            int n = axis.Height;
            Matrix I = new Matrix(n, n);
            Matrix A_t = new Matrix(n, n);

            double[] vec = new double[n];

            for (int i = 0; i < n; i++)
            {
                vec[i] = 1;
            }

            //axis.AddColumnVector(vec);

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

            Matrix a = A_t * A_t * (1 - Trigonometry.Cos(angle, true));
            Matrix b = A_t * Trigonometry.Sin(angle, true);

            return I * a + b;
        }
    }
}
