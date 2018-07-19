using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedMath.Utils
{
    public class MultiIndex : IEnumerable<ReadOnlyCollection<int>>
    {
        private int[] _indices;
        public ReadOnlyCollection<int> Indices { get; private set; }

        private int[] _fromInclusive;
        public ReadOnlyCollection<int> FromInclusive { get; private set; }

        private int[] _toExclusive;
        public ReadOnlyCollection<int> ToExclusive { get; private set; }

        private int[] _steps;
        public ReadOnlyCollection<int> Steps { get; private set; }

        public MultiIndex(int[] toExclusiveArr)
        {
            if (toExclusiveArr == null)
            {
                throw new ArgumentNullException("The array " + nameof(toExclusiveArr) + " must not be 'null'.", (Exception)null);
            }

            int[] fromInclusiveArr = new int[toExclusiveArr.Length];
            int[] stepsArr = new int[toExclusiveArr.Length];
            for (int i = 0; i < toExclusiveArr.Length; i++)
            {
                fromInclusiveArr[i] = 0;
                stepsArr[i] = 1;
            }

            Init(fromInclusiveArr, toExclusiveArr, stepsArr);
        }

        public MultiIndex(int[] fromInclusiveArr, int[] toExclusiveArr)
        {
            if (fromInclusiveArr == null || toExclusiveArr == null)
            {
                throw new ArgumentNullException("The arrays " + nameof(fromInclusiveArr) + " and " + nameof(toExclusiveArr) + " must not be 'null'.", (Exception)null);
            }

            if (toExclusiveArr.Length != fromInclusiveArr.Length)
            {
                throw new ArgumentException("The arrays " + nameof(fromInclusiveArr) + " and " + nameof(toExclusiveArr) + " must be of the same length.");
            }

            int[] stepsArr = new int[fromInclusiveArr.Length];
            for (int i = 0; i < stepsArr.Length; i++)
            {
                stepsArr[i] = 1;
            }

            Init(fromInclusiveArr, toExclusiveArr, stepsArr);
        }

        public MultiIndex(int[] fromInclusiveArr, int[] toExclusiveArr, int[] stepsArr)
        {
            if (fromInclusiveArr == null || toExclusiveArr == null || stepsArr == null)
            {
                throw new ArgumentNullException("The arrays " + nameof(fromInclusiveArr) + ", " + nameof(toExclusiveArr) + " and " + nameof(stepsArr) + " must not be 'null'.", (Exception)null);
            }

            if (toExclusiveArr.Length != fromInclusiveArr.Length || stepsArr.Length != fromInclusiveArr.Length)
            {
                throw new ArgumentException("The arrays " + nameof(fromInclusiveArr) + ", " + nameof(toExclusiveArr) + " and " + nameof(stepsArr) + " must be of the same length.");
            }

            Init(fromInclusiveArr, toExclusiveArr, stepsArr);
        }

        public MultiIndex(int toExclusive, int indexLength) : this(0, toExclusive, 1, indexLength) { }

        public MultiIndex(int fromInclusive, int toExclusive, int indexLength) : this(fromInclusive, toExclusive, 1, indexLength) { }

        public MultiIndex(int fromInclusive, int toExclusive, int steps, int indexLength)
        {
            int[] fromInclusiveArr = new int[indexLength];
            int[] toExclusiveArr = new int[indexLength];
            int[] stepsArr = new int[indexLength];

            for (int i = 0; i < indexLength; i++)
            {
                fromInclusiveArr[i] = fromInclusive;
                toExclusiveArr[i] = toExclusive;
                stepsArr[i] = steps;
            }

            Init(fromInclusiveArr, toExclusiveArr, stepsArr);
        }

        private void Init(int[] fromInclusiveArr, int[] toExclusiveArr, int[] stepsArr)
        {
            _fromInclusive = new int[fromInclusiveArr.Length];
            for (int i = 0; i < _fromInclusive.Length; i++)
            {
                _fromInclusive[i] = fromInclusiveArr[i];
            }
            FromInclusive = new ReadOnlyCollection<int>(_fromInclusive);

            _toExclusive = new int[toExclusiveArr.Length];
            for (int i = 0; i < _toExclusive.Length; i++)
            {
                _toExclusive[i] = toExclusiveArr[i];
            }
            ToExclusive = new ReadOnlyCollection<int>(_toExclusive);

            _steps = new int[stepsArr.Length];
            for (int i = 0; i < _steps.Length; i++)
            {
                _steps[i] = stepsArr[i];
            }
            Steps = new ReadOnlyCollection<int>(_steps);

            _indices = new int[_fromInclusive.Length];
            for (int i = 0; i < _indices.Length; i++)
            {
                _indices[i] = _fromInclusive[i];
            }
            Indices = new ReadOnlyCollection<int>(_indices);

            ResetLastIndex();
        }

        public int this[int index]
        {
            get
            {
                return _indices[index];
            }
        }

        public bool Increment()
        {
            for (int i = _indices.Length - 1; i >= 0; i--)
            {
                if (_indices[i] < _toExclusive[i] - 1)
                {
                    _indices[i]++;
                    return true;
                }
                else
                {
                    _indices[i] = _fromInclusive[i];
                }
            }

            ResetLastIndex();

            return false;
        }

        public int ToInt()
        {
            int sum = _indices[_indices.Length - 1] - _fromInclusive[_fromInclusive.Length - 1];
            int product = 1;

            for (int i = _indices.Length - 2; i >= 0; i--)
            {
                product *= (_toExclusive[i + 1] - _fromInclusive[i + 1]);
                sum += (_indices[i] - _fromInclusive[i]) * product;
            }

            return sum;
        }

        public void Reset()
        {
            for (int i = 0; i < _indices.Length - 1; i++)
            {
                _indices[i] = _fromInclusive[i];
            }
            ResetLastIndex();
        }

        private void ResetLastIndex()
        {
            _indices[Indices.Count - 1] = _fromInclusive[_fromInclusive.Length - 1] - 1;
        }

        public IEnumerator<ReadOnlyCollection<int>> GetEnumerator()
        {
            return new MultiIndexEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
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