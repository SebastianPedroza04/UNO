// UNO.Core/Cartas.cs
namespace UNO.Core
{
    public enum ColorCarta { Rojo, Verde, Azul, Amarillo, Negro } // Negro = comodines
    public enum TipoCarta { Numero, Skip, Reverse, Draw2, Wild, WildDraw4 }

    public struct Carta
    {
        public ColorCarta Color { get; }
        public TipoCarta Tipo { get; }
        public int Valor { get; } // válido solo si Tipo = Numero (0..9)

        public Carta(ColorCarta color, TipoCarta tipo, int valor = -1)
        {
            Color = color;
            Tipo = tipo;
            Valor = valor;
        }

        public override string ToString()
        {
            return Tipo switch
            {
                TipoCarta.Numero => $"{Color} {Valor}",
                TipoCarta.Skip => $"{Color} Skip",
                TipoCarta.Reverse => $"{Color} Reverse",
                TipoCarta.Draw2 => $"{Color} +2",
                TipoCarta.Wild => "Wild",
                TipoCarta.WildDraw4 => "Wild +4",
                _ => "?"
            };
        }
    }
}
