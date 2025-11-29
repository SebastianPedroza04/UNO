# Estructuras de datos implementadas en el proyecto

Este documento explica todas las estructuras de datos programadas a mano en `UNO.Core`, su complejidad y cómo se usan dentro del juego.

---

# 1. DynamicArray<T>
Implementación propia que reemplaza a `List<T>`.

- Arreglo interno con crecimiento dinámico (doble capacidad).
- `Count`, `Capacity`, indexador, `Add`, `RemoveAt`.

| Operación | Complejidad |
|----------|-------------|
| Add      | O(1) amortizado |
| RemoveAt | O(n) |
| Acceso   | O(1) |

**Usos en el proyecto:**
- Mano del jugador (`Jugador.Mano`)
- Mazo (`Mazo`)
- Pila de descartes (`Partida`)
- Listas de adyacencia en `Graph`

---

# 2. StackArray<T>
Pila LIFO implementada sobre arreglo.

| Operación | Complejidad |
|----------|-------------|
| Push     | O(1) |
| Pop      | O(1) |
| Peek     | O(1) |

**Usos:**
- Historial de jugadas en `Partida`

---

# 3. CircularQueue<T>
Cola FIFO circular sobre arreglo.

| Operación | Complejidad |
|----------|-------------|
| Enqueue  | O(1) |
| Dequeue  | O(1) |
| Peek     | O(1) |

**Usos:**
- BFS en `GraphAlgorithms.BfsPath`

---

# 4. LinkedList<T>
Lista enlazada simple.

| Operación | Complejidad |
|----------|-------------|
| AddLast      | O(1) |
| RemoveFirst  | O(1) |
| PeekFirst    | O(1) |

**Usos:**
- Efectos acumulados (+2, +4) en `Partida`

---

# 5. BinaryHeap<T>
Montículo binario mínimo (min-heap).

| Operación    | Complejidad |
|--------------|-------------|
| Insert       | O(log n) |
| ExtractMin   | O(log n) |
| PeekMin      | O(1) |

**Usos:**
- Algoritmo A* en `GraphAlgorithms.AStarPath`

---

# 6. HashTable<TKey, TValue>
Tabla hash con direccionamiento abierto (linear probing).

| Operación | Complejidad promedio |
|----------|-----------------------|
| Add/Update | O(1) |
| ContainsKey | O(1) |
| TryGetValue | O(1) |

**Usos:**
- Contar colores en mano (`Jugador.ContarColores()`)
- Ranking de victorias (`Ranking`)

---

# 7. AvlTree<TKey, TValue>
Árbol binario de búsqueda auto-balanceado.

| Operación | Complejidad |
|----------|-------------|
| Insert   | O(log n) |
| Search   | O(log n) |
| Traverse | O(n) |

**Usos:**
- Ranking ordenado de jugadores (victorias → descendente)

---

# 8. Graph
Grafo dirigido con listas de adyacencia construidas con `DynamicArray`.

**Usos:**
- IA del bot para representar decisiones posibles

---

# 9. GraphAlgorithms
Incluye:

## BFS
- Usa `CircularQueue`
- O(V + E)

## A*
- Usa `BinaryHeap`
- O(E log V)

**Usos en el proyecto:**
- El bot simula jugadas evaluando caminos mediante A*.

---

Todas estas estructuras están implementadas **sin usar colecciones del framework**, cumpliendo completamente los requerimientos de la asignatura.
