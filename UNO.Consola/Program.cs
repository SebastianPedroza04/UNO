// UNO.Consola/Program.cs
using System;
using UNO.Core;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== UNO (Estructuras de Datos + IA + Ranking) ===\n");

        // Pedimos jugadores
        var nombres = PedirNombres();

        // Creamos partida y ranking
        var partida = new Partida(nombres);
        var ranking = new Ranking();

        // Marcamos qué jugadores son BOT (si su nombre contiene "BOT")
        bool[] esBot = new bool[nombres.Length];
        for (int i = 0; i < nombres.Length; i++)
        {
            esBot[i] = nombres[i].ToUpper().Contains("BOT");
        }

        // Bucle principal de la partida
        while (true)
        {
            // 1) Resolver efectos pendientes (Draw2 / Draw4) antes de jugar
            if (partida.ResolverEfectosDeTurnoActual())
            {
                // El efecto ya hizo robar y adelantó turno, pasamos al siguiente ciclo
                continue;
            }

            int idx = partida.Turnos.Actual();
            Jugador jugador = partida.Jugadores[idx];

            Console.WriteLine($"\nTurno de {jugador.Nombre}");
            Console.WriteLine($"Tope: {partida.TopeDescartes()} | ColorActual: {partida.ColorActual}");

            MostrarMano(jugador);
            MostrarResumenColores(jugador);

            // 2) ¿Es jugador humano o BOT?
            if (esBot[idx])
            {
                // --- IA ---
                ColorCarta? elegidoBot;
                int cartaIdx = JugadorIA.ElegirCarta(partida, idx, out elegidoBot);

                if (cartaIdx == -1)
                {
                    Console.WriteLine($"[BOT] {jugador.Nombre} decide robar.");
                    partida.RobarN(idx, 1);
                    partida.Turnos.Avanzar();
                    continue;
                }
                else
                {
                    Carta candidata = jugador.Mano[cartaIdx];
                    Console.WriteLine($"[BOT] {jugador.Nombre} juega: {candidata}");

                    if (partida.IntentarJugar(idx, cartaIdx, elegidoBot, out string efectoBot))
                    {
                        Console.WriteLine($"[BOT] Efecto: {efectoBot}");

                        if (partida.Gano(idx))
                        {
                            Console.WriteLine($"\n¡¡{jugador.Nombre} (BOT) ganó la partida!!");
                            ranking.RegistrarVictoria(jugador.Nombre);
                            ranking.MostrarRanking();
                            break;
                        }

                        // Para +2 / +4 el avance de turno ya se hizo dentro de Partida.IntentarJugar.
                        if (candidata.Tipo != TipoCarta.Draw2 && candidata.Tipo != TipoCarta.WildDraw4)
                            partida.Turnos.Avanzar();
                    }
                    else
                    {
                        // No debería pasar, pero por seguridad:
                        Console.WriteLine("[BOT] Jugada inválida inesperada, roba una carta.");
                        partida.RobarN(idx, 1);
                        partida.Turnos.Avanzar();
                    }

                    continue;
                }
            }
            else
            {
                // --- JUGADOR HUMANO ---
                Console.Write("[c]arta # | [r]obar | [h]istorial | [q]uit: ");
                string? inp = Console.ReadLine()?.Trim().ToLower();

                if (inp == "q")
                    break;

                if (inp == "h")
                {
                    MostrarHistorial(partida, 5);
                    continue;
                }

                if (inp == "r")
                {
                    try
                    {
                        partida.RobarN(idx, 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al robar: {ex.Message}");
                    }
                    partida.Turnos.Avanzar();
                    continue;
                }

                if (inp != null && inp.StartsWith("c"))
                {
                    if (!int.TryParse(inp.Substring(1).Trim(), out int pos))
                    {
                        Console.WriteLine("Índice inválido.");
                        continue;
                    }

                    if (pos < 0 || pos >= jugador.Mano.Count)
                    {
                        Console.WriteLine("No existe esa carta.");
                        continue;
                    }

                    Carta candidata = jugador.Mano[pos];
                    ColorCarta? elegido = null;

                    if (candidata.Color == ColorCarta.Negro)
                    {
                        Console.Write("Elige color (R/V/A/Z): ");
                        string? t = Console.ReadLine()?.Trim().ToUpper();
                        elegido = t switch
                        {
                            "R" => ColorCarta.Rojo,
                            "V" => ColorCarta.Verde,
                            "A" => ColorCarta.Azul,
                            "Z" => ColorCarta.Amarillo,
                            _ => ColorCarta.Rojo
                        };
                    }

                    if (partida.IntentarJugar(idx, pos, elegido, out string efecto))
                    {
                        Console.WriteLine($"Jugada OK ({efecto})");

                        if (partida.Gano(idx))
                        {
                            Console.WriteLine($"\n¡¡{jugador.Nombre} grita UNO y GANÓ!!");
                            ranking.RegistrarVictoria(jugador.Nombre);
                            ranking.MostrarRanking();
                            break;
                        }

                        // Para +2 / +4 ya se adelantó turno dentro de Partida.IntentarJugar
                        if (candidata.Tipo != TipoCarta.Draw2 && candidata.Tipo != TipoCarta.WildDraw4)
                            partida.Turnos.Avanzar();
                    }
                    else
                    {
                        Console.WriteLine("Jugada inválida.");
                    }

                    continue;
                }

                Console.WriteLine("Comando no reconocido.");
            }
        }

        Console.WriteLine("\nFin de la partida.");
    }

    // ===================== MÉTODOS AUXILIARES =====================

    static string[] PedirNombres()
    {
        Console.Write("¿Cuántos jugadores? (2-6): ");
        int n = int.Parse(Console.ReadLine() ?? "2");
        if (n < 2) n = 2;
        if (n > 6) n = 6;

        string[] nm = new string[n];
        for (int i = 0; i < n; i++)
        {
            Console.Write($"Nombre J{i + 1}: ");
            nm[i] = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrEmpty(nm[i]))
                nm[i] = $"J{i + 1}";
        }

        Console.WriteLine("\nTIP: cualquier jugador cuyo nombre contenga 'BOT' será controlado por la IA.");
        return nm;
    }

    static void MostrarMano(Jugador j)
    {
        for (int i = 0; i < j.Mano.Count; i++)
            Console.WriteLine($"  c{i}: {j.Mano[i]}");
    }

    static void MostrarResumenColores(Jugador j)
    {
        var tabla = j.ContarColores();
        Console.Write("Resumen colores: ");
        bool primero = true;
        tabla.ForEach((color, cnt) =>
        {
            if (!primero) Console.Write(" | ");
            Console.Write($"{color}: {cnt}");
            primero = false;
        });
        Console.WriteLine();
    }

    static void MostrarHistorial(Partida p, int maxJugadas)
    {
        Console.WriteLine("Últimas jugadas:");
        int n = Math.Min(maxJugadas, p.Historial.Count);

        var temp = new StackArray<Carta>();

        // Pasamos de Historial a temp
        for (int i = 0; i < n; i++)
            temp.Push(p.Historial.Pop());

        // Volvemos sacando de temp (orden correcto) y reponiendo en Historial
        for (int i = 0; i < n; i++)
        {
            Carta c = temp.Pop();
            Console.WriteLine($"  {c}");
            p.Historial.Push(c);
        }

        if (n == 0)
            Console.WriteLine("  (sin jugadas aún)");
    }
}


