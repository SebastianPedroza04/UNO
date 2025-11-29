# Proyecto UNO – Implementación con Estructuras de Datos desde Cero

Este repositorio contiene una implementación completa del juego **UNO** en C#, utilizando exclusivamente **estructuras de datos creadas a mano**, sin emplear listas, colas ni pilas del framework (`List<T>`, `Queue<T>`, `Stack<T>`).

El objetivo del proyecto es demostrar el dominio del curso de **Estructuras de Datos**, aplicándolo a un juego real con:

- Arreglos dinámicos  
- Pilas  
- Colas circulares  
- Listas enlazadas  
- Árboles AVL  
- Montículos binarios (heaps)  
- Tablas hash  
- Grafos dirigidos  
- Algoritmos de búsqueda: BFS y A*

Además, incluye una interfaz gráfica completa en **WPF** y una versión jugable en consola.

---

## 🎮 Proyectos incluidos

### **UNO.Core**
Lógica del juego + todas las estructuras de datos implementadas desde cero.

Incluye:

- Modelos: `Carta`, `Jugador`, `Partida`, `Mazo`, `Reglas`, `GestorTurnos`.
- IA de bots (`JugadorIA`) usando grafos y heurísticas.
- Ranking con AVL + HashTable.
- **Estructuras de datos personalizadas:**
  - `DynamicArray<T>`
  - `StackArray<T>`
  - `CircularQueue<T>`
  - `LinkedList<T>`
  - `BinaryHeap<T>`
  - `HashTable<TKey,TValue>`
  - `AvlTree<TKey,TValue>`
  - `Graph` + `GraphAlgorithms` (BFS, A*)

👉 **Explicación detallada en:**  
📄 [`docs/Estructuras.md`](Estructuras.md)

---

### **UNO.Consola**
Versión de consola para probar la lógica del juego:

- Selección de jugadores humanos y bots.
- Jugadas, robos, efectos (+2, +4, Skip, Reverse).
- Mensajes paso a paso.

---

### **UNO.VisualWpf**
Interfaz gráfica en WPF:

- Pantalla de configuración:
  - número de jugadores,
  - nombres,
  - selección de BOT.
- Mano visible con cartas coloreadas.
- Botones para:
  - jugar carta,
  - robar,
  - ver historial,
  - nueva partida.
- IA automática para bots.

---

## ▶️ Cómo ejecutar

### Consola
1. Abrir la solución `UNO.sln` en Visual Studio.  
2. Bot derecho → **UNO.Consola → Establecer como proyecto de inicio**.  
3. Ejecutar (F5).

### Interfaz WPF
1. Bot derecho → **UNO.VisualWpf → Establecer como proyecto de inicio**.  
2. Ejecutar (F5).  
3. Configurar la partida desde la ventana inicial.

---

## 📚 Documentación técnica

Toda la explicación de las estructuras de datos se encuentra aquí:

👉 [`docs/Estructuras.md`](Estructuras.md)

Ahí verás:
- Qué estructura se implementó,
- Complejidades Big-O,
- Cómo funciona interiormente,
- En qué parte del juego se usa.

---

## 🛠️ Tecnologías

- C#
- .NET / WPF
- POO (Programación orientada a objetos)
- Estructuras de datos diseñadas a mano
- Algoritmos de grafos

---

## 👤 Autor

Proyecto académico para el curso de **Estructuras de Datos**, implementado desde cero para fines educativos y demostrativos.

