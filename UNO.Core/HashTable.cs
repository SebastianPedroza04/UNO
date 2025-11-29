using System;

namespace UNO.Core
{
    public class HashTable<TKey, TValue>
    {
        private struct Entry
        {
            public bool Used;
            public bool Deleted;
            public TKey Key;
            public TValue Value;
        }

        private Entry[] _tabla;
        private int _count;
        public int Count => _count;

        public HashTable(int capacity = 16)
        {
            if (capacity < 4) capacity = 4;
            _tabla = new Entry[capacity];
            _count = 0;
        }

        private int Hash(TKey key)
        {
            return (key!.GetHashCode() & 0x7fffffff) % _tabla.Length;
        }

        private void EnsureCapacity()
        {
            if (_count * 10 < _tabla.Length * 7) return; // load factor ~0.7
            var vieja = _tabla;
            _tabla = new Entry[vieja.Length * 2];
            _count = 0;

            foreach (var e in vieja)
            {
                if (e.Used && !e.Deleted)
                    AddOrUpdate(e.Key, e.Value);
            }
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            EnsureCapacity();
            int idx = Hash(key);
            int firstDeleted = -1;

            while (true)
            {
                ref var e = ref _tabla[idx];

                if (!e.Used)
                {
                    int target = firstDeleted != -1 ? firstDeleted : idx;
                    ref var t = ref _tabla[target];
                    t.Used = true;
                    t.Deleted = false;
                    t.Key = key;
                    t.Value = value;
                    _count++;
                    return;
                }

                if (e.Used && !e.Deleted && e.Key!.Equals(key))
                {
                    e.Value = value;
                    return;
                }

                if (e.Used && e.Deleted && firstDeleted == -1)
                    firstDeleted = idx;

                idx = (idx + 1) % _tabla.Length;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int idx = Hash(key);
            int start = idx;

            while (true)
            {
                ref var e = ref _tabla[idx];
                if (!e.Used)
                {
                    value = default!;
                    return false;
                }
                if (!e.Deleted && e.Key!.Equals(key))
                {
                    value = e.Value;
                    return true;
                }
                idx = (idx + 1) % _tabla.Length;
                if (idx == start)
                {
                    value = default!;
                    return false;
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        // Para recorrer todos los pares (lo usaremos en Ranking)
        public void ForEach(Action<TKey, TValue> action)
        {
            foreach (var e in _tabla)
            {
                if (e.Used && !e.Deleted)
                    action(e.Key, e.Value);
            }
        }
    }
}
