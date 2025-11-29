using System;
using System.Windows;

namespace UNO.VisualWpf
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnIniciar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numJugadores = ObtenerNumeroJugadores();
                var nombres = new string[numJugadores];
                var esBot = new bool[numJugadores];

                // Leemos sólo los primeros N jugadores
                LeerJugador(0, TxtJ1.Text, ChkJ1.IsChecked == true, nombres, esBot);
                if (numJugadores > 1) LeerJugador(1, TxtJ2.Text, ChkJ2.IsChecked == true, nombres, esBot);
                if (numJugadores > 2) LeerJugador(2, TxtJ3.Text, ChkJ3.IsChecked == true, nombres, esBot);
                if (numJugadores > 3) LeerJugador(3, TxtJ4.Text, ChkJ4.IsChecked == true, nombres, esBot);
                if (numJugadores > 4) LeerJugador(4, TxtJ5.Text, ChkJ5.IsChecked == true, nombres, esBot);
                if (numJugadores > 5) LeerJugador(5, TxtJ6.Text, ChkJ6.IsChecked == true, nombres, esBot);

                var main = new MainWindow(nombres, esBot);
                Application.Current.MainWindow = main;
                main.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int ObtenerNumeroJugadores()
        {
            if (CmbJugadores.SelectedItem is System.Windows.Controls.ComboBoxItem item &&
                int.TryParse(item.Content.ToString(), out int n))
            {
                if (n < 2) n = 2;
                if (n > 6) n = 6;
                return n;
            }
            return 2;
        }

        private void LeerJugador(int idx, string textoNombre, bool bot,
                                 string[] nombres, bool[] esBot)
        {
            string nombre = (textoNombre ?? "").Trim();
            if (string.IsNullOrEmpty(nombre))
                nombre = $"J{idx + 1}";

            nombres[idx] = nombre;
            esBot[idx] = bot;
        }
    }
}
