using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Utils
{
    public class MultiIndex : IEnumerable
    {
        private int[] indices;
        public ReadOnlyCollection<int> Indices { get; private set; }

        private int[] fromInclusive;
        public ReadOnlyCollection<int> FromInclusive { get; private set; }

        private int[] toExclusive;
        public ReadOnlyCollection<int> ToExclusive { get; private set; }

        private int[] steps;
        public ReadOnlyCollection<int> Steps { get; private set; }

        public MultiIndex(int[] toExclusiveArr, int[] fromInclusiveArr = null, int[] stepsArr = null)
        {
            if ((fromInclusiveArr != null && fromInclusiveArr.Length != toExclusiveArr.Length) || (stepsArr != null && fromInclusiveArr.Length != stepsArr.Length))
            {
                throw new ArgumentException("The arrays " + nameof(fromInclusiveArr) + ", " + nameof(toExclusiveArr) + " and " + nameof(stepsArr) + " must be of the same length.");
            }

            toExclusive = new int[toExclusiveArr.Length];
            for (int i = 0; i < toExclusive.Length; i++)
            {
                toExclusive[i] = toExclusiveArr[i];
            }
            ToExclusive = new ReadOnlyCollection<int>(toExclusive);

            fromInclusive = new int[toExclusive.Length];
            if (fromInclusiveArr == null)
            {
                for (int i = 0; i < fromInclusive.Length; i++)
                {
                    fromInclusive[i] = 0;
                } 
            }
            else
            {
                for (int i = 0; i < fromInclusive.Length; i++)
                {
                    fromInclusive[i] = fromInclusiveArr[i];
                }
            }
            FromInclusive = new ReadOnlyCollection<int>(fromInclusive);

            indices = new int[fromInclusive.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = fromInclusive[i];
            }
            indices[indices.Length - 1] = fromInclusive[fromInclusive.Length - 1] - 1;
            Indices = new ReadOnlyCollection<int>(indices);

            steps = new int[fromInclusive.Length];
            if (stepsArr == null)
            {
                for (int i = 0; i < steps.Length; i++)
                {
                    steps[i] = 1;
                }
            }
            else
            {
                for (int i = 0; i < steps.Length; i++)
                {
                    steps[i] = stepsArr[i];
                }
            }
            Steps = new ReadOnlyCollection<int>(steps);
        }

        public int this[int index]
        {
            get
            {
                return indices[index];
            }
        }

        public bool Increment()
        {
            for (int i = indices.Length - 1; i >= 0; i--)
            {
                if (indices[i] < toExclusive[i] - 1)
                {
                    indices[i]++;
                    return true;
                }
                else
                {
                    indices[i] = fromInclusive[i];
                }
            }

            indices[indices.Length - 1] = fromInclusive[fromInclusive.Length - 1] - 1;

            return false;
        }

        public int ToInt()
        {
            int sum = indices[indices.Length - 1] - fromInclusive[fromInclusive.Length - 1];
            int product = 1;

            for (int i = indices.Length - 2; i >= 0; i--)
            {
                product *= (toExclusive[i + 1] - fromInclusive[i + 1]);
                sum += (indices[i] - fromInclusive[i]) * product;
            }

            return sum;
        }

        public void Reset()
        {
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = fromInclusive[i];
            }
            indices[indices.Length - 1] = fromInclusive[fromInclusive.Length - 1] - 1;
        }

        public IEnumerator GetEnumerator()
        {
            return new MultiIndexEnumerator(this);
        }
    }

    public class MultiIndexEnumerator : IEnumerator<ReadOnlyCollection<int>>
    {
        private MultiIndex currentMIndex;

        public ReadOnlyCollection<int> Current => currentMIndex.Indices;

        object IEnumerator.Current => Current;

        public MultiIndexEnumerator(MultiIndex mindex)
        {
            currentMIndex = mindex;
        }

        public void Dispose()
        {
            return;
        }

        public bool MoveNext()
        {
            return currentMIndex.Increment();
        }

        public void Reset()
        {
            currentMIndex.Reset();
        }
    }
}
