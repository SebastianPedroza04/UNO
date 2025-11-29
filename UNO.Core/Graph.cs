using System;

namespace UNO.Core
{
    public class Graph
    {
        private readonly DynamicArray<DynamicArray<int>> _adj;

        public int VertexCount => _adj.Count;

        public Graph(int numVertices)
        {
            if (numVertices < 1) throw new ArgumentException("numVertices debe ser >= 1");
            _adj = new DynamicArray<DynamicArray<int>>(numVertices);
            for (int i = 0; i < numVertices; i++)
                _adj.Add(new DynamicArray<int>());
        }

        public void AddEdge(int from, int to)
        {
            if (from < 0 || from >= VertexCount || to < 0 || to >= VertexCount)
                throw new ArgumentOutOfRangeException();
            _adj[from].Add(to);
        }

        public DynamicArray<int> Neighbors(int v)
        {
            if (v < 0 || v >= VertexCount) throw new ArgumentOutOfRangeException();
            return _adj[v];
        }
    }
}
