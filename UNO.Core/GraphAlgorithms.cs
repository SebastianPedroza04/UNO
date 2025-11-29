using System;

namespace UNO.Core
{
    public static class GraphAlgorithms
    {
        // Devuelve el camino de start a goal (incluyéndolos) o vacío si no hay
        public static DynamicArray<int> BfsPath(Graph g, int start, int goal)
        {
            int n = g.VertexCount;
            var prev = new int[n];
            var visited = new bool[n];
            for (int i = 0; i < n; i++) prev[i] = -1;

            var q = new CircularQueue<int>(n + 2);
            visited[start] = true;
            q.Enqueue(start);

            while (q.Count > 0)
            {
                int v = q.Dequeue();
                if (v == goal) break;

                var neighbors = g.Neighbors(v);
                for (int i = 0; i < neighbors.Count; i++)
                {
                    int u = neighbors[i];
                    if (!visited[u])
                    {
                        visited[u] = true;
                        prev[u] = v;
                        q.Enqueue(u);
                    }
                }
            }

            var path = new DynamicArray<int>();
            if (!visited[goal]) return path; // vacío

            int cur = goal;
            while (cur != -1)
            {
                path.Add(cur);
                cur = prev[cur];
            }

            // invertir para que quede start -> ... -> goal
            for (int i = 0, j = path.Count - 1; i < j; i++, j--)
            {
                int tmp = path[i];
                path[i] = path[j];
                path[j] = tmp;
            }

            return path;
        }

        // Versión A* simple, grafo no ponderado (coste = número de aristas)
        private class NodeRec : IComparable<NodeRec>
        {
            public int V;
            public double G; // coste acumulado
            public double F; // G + heurística

            public int CompareTo(NodeRec other)
            {
                return F.CompareTo(other.F);
            }
        }

        public static DynamicArray<int> AStarPath(Graph g, int start, int goal, Func<int, double> heuristic)
        {
            int n = g.VertexCount;
            var prev = new int[n];
            var gScore = new double[n];
            var visited = new bool[n];

            for (int i = 0; i < n; i++)
            {
                prev[i] = -1;
                gScore[i] = double.PositiveInfinity;
            }

            gScore[start] = 0.0;

            var open = new BinaryHeap<NodeRec>();
            open.Insert(new NodeRec { V = start, G = 0.0, F = heuristic(start) });

            while (!open.IsEmpty)
            {
                var current = open.ExtractMin();
                int v = current.V;

                if (visited[v]) continue;
                visited[v] = true;

                if (v == goal) break;

                var neighbors = g.Neighbors(v);
                for (int i = 0; i < neighbors.Count; i++)
                {
                    int u = neighbors[i];
                    if (visited[u]) continue;

                    double tentativeG = gScore[v] + 1.0; // cada arista cuesta 1
                    if (tentativeG < gScore[u])
                    {
                        gScore[u] = tentativeG;
                        prev[u] = v;
                        double f = tentativeG + heuristic(u);
                        open.Insert(new NodeRec { V = u, G = tentativeG, F = f });
                    }
                }
            }

            var path = new DynamicArray<int>();
            if (!visited[goal]) return path;

            int cur2 = goal;
            while (cur2 != -1)
            {
                path.Add(cur2);
                cur2 = prev[cur2];
            }
            // invertir
            for (int i = 0, j = path.Count - 1; i < j; i++, j--)
            {
                int tmp = path[i];
                path[i] = path[j];
                path[j] = tmp;
            }
            return path;
        }
    }
}
