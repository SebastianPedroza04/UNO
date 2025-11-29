// UNO.Core/Partida.cs
using System;

namespace UNO.Core
{
    public class Partida
    {
        public Jugador[] Jugadores { get; }
        public Mazo Mazo { get; }
        public DynamicArray<Carta> Descartes { get; } = new DynamicArray<Carta>();
        public GestorTurnos Turnos { get; }
        public ColorCarta ColorActual { get; private set; }

        public StackArray<Carta> Historial { get; } = new StackArray<Carta>();

        // Cola de efectos: se aplican al inicio del turno del jugador objetivo
        public LinkedList<EfectoRobo> EfectosPendientes { get; } = new LinkedList<EfectoRobo>();

        public Partida(string[] nombres)
        {
            if (nombres.Length < 2 || nombres.Length > 10) throw new ArgumentException("2..10 jugadores");
            Jugadores = new Jugador[nombres.Length];
            for (int i = 0; i < nombres.Length; i++) Jugadores[i] = new Jugador(nombres[i]);

            Mazo = new Mazo();
            Mazo.Barajar();
            Turnos = new GestorTurnos(nombres.Length);

            // Repartir 7
            for (int k = 0; k < 7; k++)
                for (int j = 0; j < Jugadores.Length; j++)
                    Jugadores[j].Tomar(Mazo.Robar());

            // Voltear primera no comodín
            Carta primera;
            do { primera = Mazo.Robar(); } while (primera.Tipo == TipoCarta.Wild || primera.Tipo == TipoCarta.WildDraw4);
            Descartes.Add(primera);
            ColorActual = primera.Color;
        }

        public Carta TopeDescartes() => Descartes[Descartes.Count - 1];

        public bool IntentarJugar(int idxJugador, int idxCartaEnMano, ColorCarta? colorElegidoSiComodin, out string efecto)
        {
            efecto = "";
            var j = Jugadores[idxJugador];
            if (idxCartaEnMano < 0 || idxCartaEnMano >= j.Mano.Count) return false;
            var carta = j.Mano[idxCartaEnMano];
            var tope = TopeDescartes();

            if (!Reglas.EsJugadaValida(carta, tope, ColorActual, j.Mano)) return false;

            // Jugar
            j.Jugar(idxCartaEnMano);
            Descartes.Add(carta);
            Historial.Push(carta);

            // Actualizar color
            if (carta.Color != ColorCarta.Negro) ColorActual = carta.Color;
            else if (colorElegidoSiComodin.HasValue) ColorActual = colorElegidoSiComodin.Value;

            switch (carta.Tipo)
            {
                case TipoCarta.Skip:
                    Turnos.SaltarUno();
                    efecto = "Skip";
                    break;

                case TipoCarta.Reverse:
                    Turnos.ToggleReversa();
                    efecto = "Reverse";
                    break;

                case TipoCarta.Draw2:
                    {
                        Turnos.Avanzar(); // avanzamos al siguiente (víctima)
                        int victima = Turnos.Actual();
                        EfectosPendientes.AddLast(new EfectoRobo(victima, 2));
                        efecto = "+2 (efecto pendiente)";
                        break;
                    }

                case TipoCarta.Wild:
                    efecto = $"Wild ({ColorActual})";
                    break;

                case TipoCarta.WildDraw4:
                    {
                        Turnos.Avanzar(); // avanzamos al siguiente (víctima)
                        int victima = Turnos.Actual();
                        EfectosPendientes.AddLast(new EfectoRobo(victima, 4));
                        efecto = $"Wild +4 ({ColorActual}) efecto pendiente";
                        break;
                    }
            }
            return true;
        }

        public void RobarN(int idxJugador, int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (Mazo.Count == 0) RehacerMazoDesdeDescartes();
                Jugadores[idxJugador].Tomar(Mazo.Robar());
            }
        }

        private void RehacerMazoDesdeDescartes()
        {
            if (Descartes.Count <= 1) throw new InvalidOperationException("No hay cartas para rehacer el mazo");
            var tope = Descartes[Descartes.Count - 1];

            for (int i = 0; i < Descartes.Count - 1; i++)
                Mazo.Agregar(Descartes[i]);

            Descartes.Clear();
            Descartes.Add(tope);
            Mazo.Barajar();
        }

        public bool Gano(int idxJugador) => Jugadores[idxJugador].Mano.Count == 0;

        // Aplica efectos pendientes para el jugador del turno actual.
        // Devuelve true si el turno fue "saltado" por efecto (por ejemplo +2/+4).
        public bool ResolverEfectosDeTurnoActual()
        {
            if (EfectosPendientes.IsEmpty) return false;

            int actual = Turnos.Actual();
            var nodo = EfectosPendientes.Head;
            if (nodo == null) return false;

            var efecto = nodo.Value;
            if (efecto.JugadorObjetivo != actual) return false;

            // Es para el jugador actual: aplicar
            Console.WriteLine($">>> Efecto pendiente: {Jugadores[actual].Nombre} debe robar {efecto.Cantidad} cartas.");
            RobarN(actual, efecto.Cantidad);

            // quitar de la lista
            EfectosPendientes.RemoveFirst();

            // El jugador pierde el turno
            Turnos.Avanzar();
            return true;
        }
    }
}
