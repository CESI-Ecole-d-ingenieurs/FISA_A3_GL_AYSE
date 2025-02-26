using System;
using System.Windows;

namespace BackupMonitorClient
{
    public partial class MainWindow : Window
    {
        private ClientController clientController;

        public MainWindow()
        {
            InitializeComponent();
            clientController = new ClientController();
        }

        private async void Connection(object sender, RoutedEventArgs e)
        {
            await clientController.ConnectToServerAsync();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            clientController.SendCommand("PAUSE");
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            clientController.SendCommand("RESUME");
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            clientController.SendCommand("STOP");
        }

        private async void Exit(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => clientController.Disconnect()); // Attendre la déconnexion
            Application.Current.Shutdown(); // Fermer après la déconnexion
        }

    }
}
