// UNO.Core/GestorTurnos.cs

namespace UNO.Core
{
    public class GestorTurnos
    {
        private readonly DynamicArray<int> _orden = new();
        private int _idxActual = 0;
        private int _dir = 1; // 1 = normal, -1 = reversa

        public GestorTurnos(int numJugadores)
        {
            for (int i = 0; i < numJugadores; i++) _orden.Add(i);
        }

        public int Actual() => _orden[_idxActual];

        public void Avanzar()
        {
            int n = _orden.Count;
            _idxActual = (_idxActual + _dir + n) % n;
        }

        public void SaltarUno() => Avanzar();

        public void ToggleReversa() => _dir = -_dir;
    }
}
