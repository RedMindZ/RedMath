namespace RedMath.LinearAlgebra
{
    public class VectorSpace
    {
        private Vector[] vertices;

        public VectorSpace(int count)
        {
            vertices = new Vector[count];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector();
            }
        }

        public VectorSpace(params Vector[] vert)
        {
            vertices = new Vector[vert.Length];

            for (int i = 0; i < vert.Length; i++)
            {
                vertices[i] = vert[i];
            }
        }

        public Vector this[int vec]
        {
            get
            {
                return vertices[vec];
            }

            set
            {
                vertices[vec] = value;
            }
        }

        public double this[int vec, int comp]
        {
            get
            {
                return vertices[vec][comp];
            }

            set
            {
                vertices[vec][comp] = value;
            }
        }
    }
}
