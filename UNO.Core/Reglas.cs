// UNO.Core/Reglas.cs

namespace UNO.Core
{
    public static class Reglas
    {
        public static bool EsJugadaValida(Carta carta, Carta tope, ColorCarta colorActual, DynamicArray<Carta> manoDelJugador)
        {
            if (carta.Tipo == TipoCarta.Wild) return true;

            if (carta.Tipo == TipoCarta.WildDraw4)
            {
                // Solo si NO tiene carta del colorActual (ignorando comodines)
                return !TieneColorJugable(manoDelJugador, colorActual);
            }

            if (carta.Color == colorActual) return true;
            if (carta.Tipo == tope.Tipo && carta.Tipo != TipoCarta.Numero) return true;
            if (carta.Tipo == TipoCarta.Numero && tope.Tipo == TipoCarta.Numero && carta.Valor == tope.Valor) return true;
            return false;
        }

        private static bool TieneColorJugable(DynamicArray<Carta> mano, ColorCarta color)
        {
            for (int i = 0; i < mano.Count; i++)
            {
                var c = mano[i];
                if (c.Color == color && c.Color != ColorCarta.Negro) return true;
            }
            return false;
        }
    }
}
