// UNO.Core/Jugador.cs
namespace UNO.Core
{
    public class Jugador
    {
        public string Nombre { get; }
        public DynamicArray<Carta> Mano { get; } = new DynamicArray<Carta>();

        public Jugador(string nombre) => Nombre = nombre;

        public void Tomar(Carta c) => Mano.Add(c);

        public Carta Jugar(int indiceCarta) => Mano.RemoveAt(indiceCarta);

        public HashTable<ColorCarta, int> ContarColores()
        {
            var tabla = new HashTable<ColorCarta, int>();
            for (int i = 0; i < Mano.Count; i++)
            {
                var c = Mano[i];
                if (c.Color == ColorCarta.Negro) continue; // comodines aparte
                if (!tabla.TryGetValue(c.Color, out int cnt)) cnt = 0;
                cnt++;
                tabla.AddOrUpdate(c.Color, cnt);
            }
            return tabla;
        }
    }
}
