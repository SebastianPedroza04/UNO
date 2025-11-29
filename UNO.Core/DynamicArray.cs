// UNO.Core/DynamicArray.cs
namespace UNO.Core
{
    public class DynamicArray<T>
    {
        private T[] _data;
        public int Count { get; private set; }

        public DynamicArray(int capacity = 4)
        {
            if (capacity < 1) capacity = 4;
            _data = new T[capacity];
            Count = 0;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                return _data[index];
            }
            set
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                _data[index] = value;
            }
        }

        public void Add(T item)
        {
            EnsureCapacity(Count + 1);
            _data[Count++] = item;
        }

        public T RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            T removed = _data[index];
            for (int i = index; i < Count - 1; i++)
                _data[i] = _data[i + 1];
            Count--;
            _data[Count] = default!;
            return removed;
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++) _data[i] = default!;
            Count = 0;
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
