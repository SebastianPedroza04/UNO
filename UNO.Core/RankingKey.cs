using System;

namespace UNO.Core
{
    public struct RankingKey : IComparable<RankingKey>
    {
        public int Victorias { get; }
        public string Nombre { get; }

        public RankingKey(int victorias, string nombre)
        {
            Victorias = victorias;
            Nombre = nombre ?? "";
        }

        public int CompareTo(RankingKey other)
        {
            // Queremos que al recorrer "descendente" salgan primero más victorias,
            // así que comparamos al revés: más victorias = "mayor".
            int cmpVict = Victorias.CompareTo(other.Victorias);
            if (cmpVict != 0) return cmpVict;

            // Si empatan en victorias, orden alfabético por nombre
            return Nombre.CompareTo(other.Nombre);
        }

        public override string ToString() => $"{Nombre} ({Victorias})";
    }
}
