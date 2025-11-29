using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UNO.Core;

namespace UNO.VisualWpf
{
    public partial class MainWindow : Window
    {
        private Partida _partida;
        private Ranking _ranking;
        private bool[] _esBot;
        private string[] _nombres;

        private int? _cartaSeleccionadaIndex = null;
        private Button? _cartaSeleccionadaButton = null;
        private bool _juegoTerminado = false;

        // Constructor usado desde ConfigWindow
        public MainWindow(string[] nombres, bool[] esBot)
        {
            InitializeComponent();
            IniciarNuevaPartida(nombres, esBot);
        }

        // Constructor por defecto (útil para el diseñador, o si arrancaras directo aquí)
        public MainWindow() : this(new[] { "Tú", "BOT1" }, new[] { false, true }) { }

        // =================== INICIO / REINICIO ===================

        private void IniciarNuevaPartida(string[] nombres, bool[] esBot)
        {
            if (nombres == null || esBot == null || nombres.Length != esBot.Length)
                throw new ArgumentException("Los arreglos de nombres y bots deben tener misma longitud.");

            _nombres = nombres;
            _esBot = new bool[nombres.Length];
            Array.Copy(esBot, _esBot, nombres.Length);

            _partida = new Partida(nombres);
            _ranking = new Ranking();

            _juegoTerminado = false;
            _cartaSeleccionadaIndex = null;
            _cartaSeleccionadaButton = null;

            TxtLog.Clear();
            Log($"Nueva partida iniciada: {string.Join(", ", nombres)}.");
            ProcesarTurnos();
        }

        // =================== BUCLE DE TURNOS (EVENT-DRIVEN) ===================

        private void ProcesarTurnos()
        {
            if (_juegoTerminado) return;

            while (true)
            {
                if (_partida.ResolverEfectosDeTurnoActual())
                    continue;

                int idx = _partida.Turnos.Actual();
                Jugador jugador = _partida.Jugadores[idx];

                if (_esBot[idx])
                {
                    EjecutarTurnoBot(idx, jugador);
                    if (_juegoTerminado) return;
                }
                else
                {
                    RenderizarEstado();
                    return;
                }
            }
        }

        private void EjecutarTurnoBot(int idx, Jugador jugador)
        {
            Log($"[BOT] Turno de {jugador.Nombre}.");
            ColorCarta? elegido;
            int cartaIdx = JugadorIA.ElegirCarta(_partida, idx, out elegido);

            if (cartaIdx == -1)
            {
                Log($"[BOT] {jugador.Nombre} decide robar una carta.");
                _partida.RobarN(idx, 1);
                _partida.Turnos.Avanzar();
                return;
            }

            var carta = jugador.Mano[cartaIdx];
            Log($"[BOT] {jugador.Nombre} juega: {carta}");

            if (_partida.IntentarJugar(idx, cartaIdx, elegido, out string efecto))
            {
                if (!string.IsNullOrEmpty(efecto))
                    Log($"[BOT] Efecto: {efecto}");

                if (_partida.Gano(idx))
                {
                    Log($"¡¡{jugador.Nombre} (BOT) ganó la partida!!");
                    MessageBox.Show($"{jugador.Nombre} (BOT) ganó la partida.", "Fin",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _ranking.RegistrarVictoria(jugador.Nombre);
                    _ranking.MostrarRanking();
                    _juegoTerminado = true;
                    DeshabilitarControles();
                    return;
                }

                if (carta.Tipo != TipoCarta.Draw2 && carta.Tipo != TipoCarta.WildDraw4)
                    _partida.Turnos.Avanzar();
            }
            else
            {
                Log("[BOT] Jugada inesperadamente inválida, roba una carta.");
                _partida.RobarN(idx, 1);
                _partida.Turnos.Avanzar();
            }
        }

        // =================== RENDERIZADO ===================

        private void RenderizarEstado()
        {
            int idx = _partida.Turnos.Actual();
            Jugador jugador = _partida.Jugadores[idx];

            LblTurno.Text = $"Turno de: {jugador.Nombre}";
            LblTope.Text = $"Tope descartes: {_partida.TopeDescartes()}";
            LblColorActual.Text = $"Color actual: {_partida.ColorActual}";

            _cartaSeleccionadaIndex = null;
            _cartaSeleccionadaButton = null;

            PanelMano.Children.Clear();
            for (int i = 0; i < jugador.Mano.Count; i++)
            {
                var carta = jugador.Mano[i];
                var btn = CrearBotonCarta(carta, i);
                PanelMano.Children.Add(btn);
            }

            BtnJugar.IsEnabled = true;
            BtnRobar.IsEnabled = true;
            BtnHistorial.IsEnabled = true;
        }

        private Button CrearBotonCarta(Carta carta, int index)
        {
            var btn = new Button
            {
                Margin = new Thickness(4),
                Padding = new Thickness(8, 4, 8, 4),
                Tag = index,
                Content = carta.ToString(),
                MinWidth = 80,
                MinHeight = 50,
                FontWeight = FontWeights.Bold
            };

            btn.Background = new SolidColorBrush(ColorDeCarta(carta.Color));
            btn.Foreground = Brushes.White;
            btn.BorderBrush = Brushes.White;
            btn.BorderThickness = new Thickness(1);

            btn.Click += CartaButton_Click;
            return btn;
        }

        private Color ColorDeCarta(ColorCarta color)
        {
            return color switch
            {
                ColorCarta.Rojo => Colors.Red,
                ColorCarta.Verde => Colors.Green,
                ColorCarta.Azul => Colors.SteelBlue,
                ColorCarta.Amarillo => Colors.Goldenrod,
                ColorCarta.Negro => Colors.Black,
                _ => Colors.Gray
            };
        }

        // =================== EVENTOS DE UI ===================

        private void CartaButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (!int.TryParse(btn.Tag?.ToString(), out int idx)) return;

            if (_cartaSeleccionadaButton != null)
            {
                _cartaSeleccionadaButton.BorderThickness = new Thickness(1);
                _cartaSeleccionadaButton.BorderBrush = Brushes.White;
            }

            _cartaSeleccionadaButton = btn;
            _cartaSeleccionadaIndex = idx;

            btn.BorderThickness = new Thickness(3);
            btn.BorderBrush = Brushes.LimeGreen;

            Log($"Carta seleccionada: {btn.Content}");
        }

        private void BtnJugar_Click(object sender, RoutedEventArgs e)
        {
            if (_juegoTerminado) return;

            int idxJugador = _partida.Turnos.Actual();
            Jugador jugador = _partida.Jugadores[idxJugador];

            if (_esBot[idxJugador])
            {
                Log("Es turno del BOT, no puedes jugar manualmente.");
                return;
            }

            if (_cartaSeleccionadaIndex == null)
            {
                MessageBox.Show("Primero selecciona una carta de tu mano.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int pos = _cartaSeleccionadaIndex.Value;
            if (pos < 0 || pos >= jugador.Mano.Count)
            {
                MessageBox.Show("La carta seleccionada ya no existe.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var carta = jugador.Mano[pos];
            ColorCarta? elegido = null;

            if (carta.Color == ColorCarta.Negro)
            {
                elegido = ElegirColorDominante(jugador);
                Log($"Has jugado comodín, color elegido automáticamente: {elegido}");
            }

            if (_partida.IntentarJugar(idxJugador, pos, elegido, out string efecto))
            {
                Log($"Has jugado: {carta}. Efecto: {efecto}");

                if (_partida.Gano(idxJugador))
                {
                    Log("¡¡Has ganado la partida!!");
                    MessageBox.Show("¡¡Has ganado la partida!!", "Victoria",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _ranking.RegistrarVictoria(jugador.Nombre);
                    _ranking.MostrarRanking();
                    _juegoTerminado = true;
                    DeshabilitarControles();
                    return;
                }

                if (carta.Tipo != TipoCarta.Draw2 && carta.Tipo != TipoCarta.WildDraw4)
                    _partida.Turnos.Avanzar();

                ProcesarTurnos();
            }
            else
            {
                Log("Jugada inválida.");
            }
        }

        private void BtnRobar_Click(object sender, RoutedEventArgs e)
        {
            if (_juegoTerminado) return;

            int idxJugador = _partida.Turnos.Actual();
            Jugador jugador = _partida.Jugadores[idxJugador];

            if (_esBot[idxJugador])
            {
                Log("Es turno del BOT, no puedes robar manualmente.");
                return;
            }

            _partida.RobarN(idxJugador, 1);
            Log($"{jugador.Nombre} roba una carta.");
            _partida.Turnos.Avanzar();
            ProcesarTurnos();
        }

        private void BtnHistorial_Click(object sender, RoutedEventArgs e)
        {
            if (_partida.Historial.Count == 0)
            {
                Log("Historial vacío.");
                return;
            }

            int maxJugadas = 8;
            int n = Math.Min(maxJugadas, _partida.Historial.Count);
            var temp = new StackArray<Carta>();

            Log("Últimas jugadas:");
            for (int i = 0; i < n; i++)
                temp.Push(_partida.Historial.Pop());

            for (int i = 0; i < n; i++)
            {
                var c = temp.Pop();
                Log($"  {c}");
                _partida.Historial.Push(c);
            }
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            // Nueva partida con la misma configuración (nombres y bots)
            IniciarNuevaPartida(_nombres, _esBot);
        }

        // =================== UTILIDADES ===================

        private ColorCarta ElegirColorDominante(Jugador j)
        {
            var tabla = j.ContarColores();
            ColorCarta mejorColor = ColorCarta.Rojo;
            int mejorCnt = -1;
            tabla.ForEach((col, cnt) =>
            {
                if (cnt > mejorCnt)
                {
                    mejorCnt = cnt;
                    mejorColor = col;
                }
            });
            return mejorCnt > 0 ? mejorColor : ColorCarta.Rojo;
        }

        private void Log(string mensaje)
        {
            TxtLog.AppendText(mensaje + Environment.NewLine);
            TxtLog.ScrollToEnd();
        }

        private void DeshabilitarControles()
        {
            BtnJugar.IsEnabled = false;
            BtnRobar.IsEnabled = false;
            BtnHistorial.IsEnabled = false;
        }
    }
}
