using System;

namespace UNO.Core
{
    public class Ranking
    {
        private readonly HashTable<string, int> _victorias = new HashTable<string, int>();

        public void RegistrarVictoria(string nombre)
        {
            if (!_victorias.TryGetValue(nombre, out int v))
                v = 0;
            v++;
            _victorias.AddOrUpdate(nombre, v);
        }

        public void MostrarRanking()
        {
            Console.WriteLine("\n=== RANKING DE JUGADORES (por victorias) ===");

            var arbol = new AvlTree<RankingKey, object?>();

            _victorias.ForEach((nombre, victorias) =>
            {
                var key = new RankingKey(victorias, nombre);
                arbol.Insert(key, null);
            });

            // Recorrido descendente: más victorias primero
            arbol.TraverseDescending((key, _) =>
            {
                Console.WriteLine($"  {key.Nombre}: {key.Victorias} victorias");
            });

            Console.WriteLine("============================================\n");
        }
    }
}

