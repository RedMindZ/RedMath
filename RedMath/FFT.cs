namespace RedMath
{
    public class FFT
    {
        private Complex[] sourceData;

        public int BinaryLog
        {
            get;
            private set;
        }


        public Complex[] SourceData
        {
            get
            {
                return sourceData;
            }

            set
            {
                if (!Algebra.IsPowerOfTwo((ulong)value.Length))
                {
                    sourceData = new Complex[Algebra.NextGreaterPowerOfTwo(value.Length)];

                    for (int i = value.Length; i < sourceData.Length; i++)
                    {
                        sourceData[i] = new Complex();
                    }
                }
                else
                {
                    sourceData = new Complex[value.Length];
                }

                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == null)
                    {
                        sourceData[i] = new Complex();
                    }
                    else
                    {
                        sourceData[i] = value[i];
                    }
                }

                TransformedData = new Complex[sourceData.Length];

                BinaryLog = (int)System.Math.Log(TransformedData.Length, 2);

                Transform();
            }
        }

        public Complex[] TransformedData
        {
            get;
            protected set;
        }

        public FFT(int size)
        {
            SourceData = new Complex[size];
        }

        public FFT(Complex[] data)
        {
            SourceData = data;
        }

        public void Transform()
        {
            for (int i = 0; i < sourceData.Length; i++)
            {
                TransformedData[i] = sourceData[i];
            }

            computeFFT(TransformedData);
        }

        public double Amplitude(int index)
        {
            return TransformedData[index].AbsoluteValue / TransformedData.Length;
        }

        public double Phase(int index)
        {
            return TransformedData[index].Phase;
        }

        protected Complex[] getEvenIndices(Complex[] arr)
        {
            Complex[] res;

            if (arr.Length % 2 == 0)
            {
                res = new Complex[arr.Length / 2];
            }
            else
            {
                res = new Complex[arr.Length / 2 + 1];
            }

            for (int i = 0; i < arr.Length; i += 2)
            {
                res[i / 2] = arr[i];
            }

            return res;
        }

        protected Complex[] getOddIndices(Complex[] arr)
        {
            Complex[] res = new Complex[arr.Length / 2];

            for (int i = 1; i < arr.Length; i += 2)
            {
                res[i / 2] = arr[i];
            }

            return res;
        }

        public Complex[] computeFFT(Complex[] data)
        {
            if (data.Length == 1)
                return data;

            Complex[] even = computeFFT(getEvenIndices(data));
            Complex[] odd = computeFFT(getOddIndices(data));

            int halfLength = data.Length / 2;

            System.Array.Copy(even, data, even.Length);
            System.Array.Copy(odd, 0, data, halfLength, odd.Length);

            Complex ePow;
            Complex t;
            double Im = -2 * Algebra.PI / data.Length;
            for (int i = 0; i < halfLength; i++)
            {
                t = data[i];
                ePow = Algebra.Exponent(new Complex(0, Im * i)) * data[i + halfLength];
                data[i] = t + ePow;
                data[i + halfLength] = t - ePow;
            }

            return data;
            /*
            int sliceSize = 2;
            int k;
            int halfLength = data.Length / 2;
            Complex ePow;
            Complex t;
            double Im = -2 * Algebra.PI / data.Length;

            arrangeForTransformation(data);

            while (sliceSize <= data.Length)
            {
                k = sliceSize - 1;

                while (k < data.Length)
                {
                    halfLength = sliceSize / 2;
                    /*for (int i = 0; i < halfLength; i++)
                    {
                        t = data[i];
                        ePow = Algebra.Exponent(new Complex(0, Im * i)) * data[i + halfLength];
                        data[i] = t + ePow;
                        data[i + halfLength] = t - ePow;
                    }
                    combine(data, k - sliceSize + 1, k);
                    k += sliceSize;
                }

                sliceSize *= 2;
            }*/
        }
    }
}
