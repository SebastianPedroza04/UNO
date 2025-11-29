// UNO.Core/CircularQueue.cs
namespace UNO.Core
{
    public class CircularQueue<T>
    {
        private T[] _data;
        private int _front; // índice del primer elemento
        private int _size;

        public int Count => _size;

        public CircularQueue(int capacity = 8)
        {
            if (capacity < 1) capacity = 8;
            _data = new T[capacity];
            _front = 0;
            _size = 0;
        }

        public void Enqueue(T item)
        {
            EnsureCapacity(_size + 1);
            int back = (_front + _size) % _data.Length;
            _data[back] = item;
            _size++;
        }

        public T Dequeue()
        {
            if (_size == 0) throw new InvalidOperationException("Empty queue");
            T item = _data[_front];
            _data[_front] = default!;
            _front = (_front + 1) % _data.Length;
            _size--;
            return item;
        }

        public T Peek()
        {
            if (_size == 0) throw new InvalidOperationException("Empty queue");
            return _data[_front];
        }

        private void EnsureCapacity(int needed)
        {
            if (needed <= _data.Length) return;
            int newCap = Math.Max(_data.Length * 2, needed);
            var nd = new T[newCap];
            for (int i = 0; i < _size; i++)
                nd[i] = _data[(_front + i) % _data.Length];
            _data = nd;
            _front = 0;
        }
    }
}
