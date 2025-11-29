using System;

namespace UNO.Core
{
    internal class AvlNode<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public AvlNode<TKey, TValue>? Left;
        public AvlNode<TKey, TValue>? Right;
        public int Height;

        public AvlNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Height = 1;
        }
    }

    public class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private AvlNode<TKey, TValue>? _root;

        private int Height(AvlNode<TKey, TValue>? n) => n?.Height ?? 0;

        private int BalanceFactor(AvlNode<TKey, TValue>? n) => n == null ? 0 : Height(n.Left) - Height(n.Right);

        private void UpdateHeight(AvlNode<TKey, TValue> n)
        {
            n.Height = Math.Max(Height(n.Left), Height(n.Right)) + 1;
        }

        private AvlNode<TKey, TValue> RotateRight(AvlNode<TKey, TValue> y)
        {
            var x = y.Left!;
            var T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            UpdateHeight(y);
            UpdateHeight(x);

            return x;
        }

        private AvlNode<TKey, TValue> RotateLeft(AvlNode<TKey, TValue> x)
        {
            var y = x.Right!;
            var T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            UpdateHeight(x);
            UpdateHeight(y);

            return y;
        }

        private AvlNode<TKey, TValue> Insert(AvlNode<TKey, TValue>? node, TKey key, TValue value)
        {
            if (node == null) return new AvlNode<TKey, TValue>(key, value);

            int cmp = key.CompareTo(node.Key);
            if (cmp < 0)
                node.Left = Insert(node.Left, key, value);
            else if (cmp > 0)
                node.Right = Insert(node.Right, key, value);
            else
            {
                // llave igual -> actualizamos valor
                node.Value = value;
                return node;
            }

            UpdateHeight(node);

            int bf = BalanceFactor(node);

            // Left Left
            if (bf > 1 && key.CompareTo(node.Left!.Key) < 0)
                return RotateRight(node);

            // Right Right
            if (bf < -1 && key.CompareTo(node.Right!.Key) > 0)
                return RotateLeft(node);

            // Left Right
            if (bf > 1 && key.CompareTo(node.Left!.Key) > 0)
            {
                node.Left = RotateLeft(node.Left!);
                return RotateRight(node);
            }

            // Right Left
            if (bf < -1 && key.CompareTo(node.Right!.Key) < 0)
            {
                node.Right = RotateRight(node.Right!);
                return RotateLeft(node);
            }

            return node;
        }

        public void Insert(TKey key, TValue value)
        {
            _root = Insert(_root, key, value);
        }

        // Recorrido in-order descendente (para ranking mayor a menor)
        public void TraverseDescending(Action<TKey, TValue> action)
        {
            TraverseDescending(_root, action);
        }

        private void TraverseDescending(AvlNode<TKey, TValue>? node, Action<TKey, TValue> action)
        {
            if (node == null) return;
            TraverseDescending(node.Right, action);
            action(node.Key, node.Value);
            TraverseDescending(node.Left, action);
        }
    }
}
