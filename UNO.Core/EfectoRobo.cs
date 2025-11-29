namespace UNO.Core
{
    public class EfectoRobo
    {
        public int JugadorObjetivo { get; }
        public int Cantidad { get; }

        public EfectoRobo(int jugadorObjetivo, int cantidad)
        {
            JugadorObjetivo = jugadorObjetivo;
            Cantidad = cantidad;
        }

        public override string ToString() => $"Jugador {JugadorObjetivo} debe robar {Cantidad}";
    }
}
