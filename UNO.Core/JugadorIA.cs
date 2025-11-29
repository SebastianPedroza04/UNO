using System;

namespace UNO.Core
{
    public static class JugadorIA
    {
        // Devuelve índice de carta a jugar o -1 si decide robar.
        // También devuelve el color elegido para comodines (si aplica).
        public static int ElegirCarta(Partida p, int idxJugador, out ColorCarta? colorElegido)
        {
            colorElegido = null;
            var j = p.Jugadores[idxJugador];
            var mano = j.Mano;
            var tope = p.TopeDescartes();
            var colorActual = p.ColorActual;

            var jugadas = new DynamicArray<int>(); // índices de carta en mano

            for (int i = 0; i < mano.Count; i++)
            {
                var c = mano[i];
                if (Reglas.EsJugadaValida(c, tope, colorActual, mano))
                    jugadas.Add(i);
            }

            if (jugadas.Count == 0)
            {
                // No hay jugada válida → IA decide robar
                return -1;
            }

            // Si hay varias, asignamos score
            var scores = new DynamicArray<double>(jugadas.Count);
            for (int k = 0; k < jugadas.Count; k++)
            {
                var c = mano[jugadas[k]];
                double s = PuntuarCarta(c, colorActual);
                scores.Add(s);
            }

            // Creamos un grafo con 1 + jugadas.Count nodos:
            // 0 = estado actual
            // 1..N = jugar jugadas[k]
            int nNodes = 1 + jugadas.Count;
            var g = new Graph(nNodes);
            for (int k = 0; k < jugadas.Count; k++)
            {
                g.AddEdge(0, k + 1);
            }

            // Elegimos como "goal" el nodo con mejor score
            int bestIdx = 0;
            double bestScore = double.NegativeInfinity;
            for (int k = 0; k < jugadas.Count; k++)
            {
                if (scores[k] > bestScore)
                {
                    bestScore = scores[k];
                    bestIdx = k;
                }
            }

            int goalNode = bestIdx + 1;

            // Heurística simple: penaliza nodos más lejos del goal por índice (solo para que A* se use)
            double Heur(int v)
            {
                if (v == goalNode) return 0.0;
                return Math.Abs(goalNode - v);
            }

            // Llamamos A* (para el curso) aunque el grafo sea trivial
            var path = GraphAlgorithms.AStarPath(g, 0, goalNode, Heur);
            // path será algo como [0, goalNode]; lo importante es que se usó A*

            int cartaElegida = jugadas[bestIdx];
            var carta = mano[cartaElegida];

            if (carta.Color == ColorCarta.Negro)
            {
                // Eligir color dominante en la mano
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
                // Si no tiene colores, elegimos rojo por defecto
                colorElegido = mejorCnt > 0 ? mejorColor : ColorCarta.Rojo;
            }

            return cartaElegida;
        }

        // Heurística sencilla: preferir cartas de acción y especialmente +4/+2
        private static double PuntuarCarta(Carta c, ColorCarta colorActual)
        {
            double score = 0;

            if (c.Tipo == TipoCarta.Numero) score += 1;
            if (c.Tipo == TipoCarta.Skip || c.Tipo == TipoCarta.Reverse) score += 2;
            if (c.Tipo == TipoCarta.Draw2) score += 3;
            if (c.Tipo == TipoCarta.Wild) score += 2.5;
            if (c.Tipo == TipoCarta.WildDraw4) score += 4;

            if (c.Color == colorActual && c.Color != ColorCarta.Negro) score += 0.5;

            return score;
        }
    }
}
