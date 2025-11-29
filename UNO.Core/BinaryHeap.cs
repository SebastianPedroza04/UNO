using System;

namespace UNO.Core
{
    // Min-heap genérico
    public class BinaryHeap<T> where T : IComparable<T>
    {
        private T[] _data;
        public int Count { get; private set; }

        public BinaryHeap(int capacity = 16)
        {
            if (capacity < 1) capacity = 16;
            _data = new T[capacity];
            Count = 0;
        }

        public void Insert(T item)
        {
            EnsureCapacity(Count + 1);
            _data[Count] = item;
            SiftUp(Count);
            Count++;
        }

        public T ExtractMin()
        {
            if (Count == 0) throw new InvalidOperationException("Heap vacío");
            T min = _data[0];
            Count--;
            _data[0] = _data[Count];
            _data[Count] = default!;
            SiftDown(0);
            return min;
        }

        public T PeekMin()
        {
            if (Count == 0) throw new InvalidOperationException("Heap vacío");
            return _data[0];
        }

        public bool IsEmpty => Count == 0;

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (_data[i].CompareTo(_data[parent]) >= 0) break;
                Swap(i, parent);
                i = parent;
            }
        }

        private void SiftDown(int i)
        {
            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int smallest = i;

                if (left < Count && _data[left].CompareTo(_data[smallest]) < 0) smallest = left;
                if (right < Count && _data[right].CompareTo(_data[smallest]) < 0) smallest = right;

                if (smallest == i) break;
                Swap(i, smallest);
                i = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            T tmp = _data[a];
            _data[a] = _data[b];
            _data[b] = tmp;
        }

        private void EnsureCapacity(int needed)
        {
            if (needed <= _data.Length) return;
            int newCap = Math.Max(_data.Length * 2, needed);
            var nd = new T[newCap];
            Array.Copy(_data, nd, Count);
            _data = nd;
        }
    }
}
