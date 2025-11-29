namespace UNO.Core
{
    public class StackArray<T>
    {
        private T[] _data;
        public int Count { get; private set; }

        public StackArray(int capacity = 8)
        {
            if (capacity < 1) capacity = 8;
            _data = new T[capacity];
            Count = 0;
        }

        public void Push(T item)
        {
            EnsureCapacity(Count + 1);
            _data[Count++] = item;
        }

        public T Pop()
        {
            if (Count == 0) throw new InvalidOperationException("Empty stack");
            var x = _data[--Count];
            _data[Count] = default!;
            return x;
        }

        public T Peek()
        {
            if (Count == 0) throw new InvalidOperationException("Empty stack");
            return _data[Count - 1];
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
