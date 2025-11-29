// UNO.Core/Mazo.cs

namespace UNO.Core
{
    public class Mazo
    {
        private readonly DynamicArray<Carta> _cartas = new();

        public int Count => _cartas.Count;

        public Mazo(bool incluirComodines = true)
        {
            var colores = new[] { ColorCarta.Rojo, ColorCarta.Verde, ColorCarta.Azul, ColorCarta.Amarillo };

            foreach (var c in colores)
            {
                _cartas.Add(new Carta(c, TipoCarta.Numero, 0));
                for (int v = 1; v <= 9; v++) { _cartas.Add(new Carta(c, TipoCarta.Numero, v)); _cartas.Add(new Carta(c, TipoCarta.Numero, v)); }
                _cartas.Add(new Carta(c, TipoCarta.Skip)); _cartas.Add(new Carta(c, TipoCarta.Skip));
                _cartas.Add(new Carta(c, TipoCarta.Reverse)); _cartas.Add(new Carta(c, TipoCarta.Reverse));
                _cartas.Add(new Carta(c, TipoCarta.Draw2)); _cartas.Add(new Carta(c, TipoCarta.Draw2));
            }
            if (incluirComodines)
            {
                for (int i = 0; i < 4; i++) _cartas.Add(new Carta(ColorCarta.Negro, TipoCarta.Wild));
                for (int i = 0; i < 4; i++) _cartas.Add(new Carta(ColorCarta.Negro, TipoCarta.WildDraw4));
            }
        }

        public void Barajar(int seed = 0)
        {
            var rnd = seed == 0 ? new Random() : new Random(seed);
            for (int i = Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                var tmp = _cartas[i]; _cartas[i] = _cartas[j]; _cartas[j] = tmp;
            }
        }

        public Carta Robar()
        {
            if (Count == 0) throw new InvalidOperationException("Mazo vacío");
            return _cartas.RemoveAt(Count - 1);
        }

        // Para rehacer mazo desde descartes
        public void Agregar(Carta c) => _cartas.Add(c);
    }
}

