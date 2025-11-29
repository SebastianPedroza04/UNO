using System;

namespace UNO.Core
{
    public class LinkedListNode<T>
    {
        public T Value;
        public LinkedListNode<T>? Next;

        public LinkedListNode(T value)
        {
            Value = value;
            Next = null;
        }
    }

    public class LinkedList<T>
    {
        public LinkedListNode<T>? Head { get; private set; }
        public LinkedListNode<T>? Tail { get; private set; }
        public int Count { get; private set; }

        public void AddLast(T value)
        {
            var node = new LinkedListNode<T>(value);
            if (Head == null)
            {
                Head = Tail = node;
            }
            else
            {
                Tail!.Next = node;
                Tail = node;
            }
            Count++;
        }

        public T RemoveFirst()
        {
            if (Head == null) throw new InvalidOperationException("Lista vacía");
            var v = Head.Value;
            Head = Head.Next;
            if (Head == null) Tail = null;
            Count--;
            return v;
        }

        public T PeekFirst()
        {
            if (Head == null) throw new InvalidOperationException("Lista vacía");
            return Head.Value;
        }

        public bool IsEmpty => Count == 0;
    }
}
