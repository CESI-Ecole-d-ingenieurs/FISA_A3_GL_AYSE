using System;
using System.Windows;

namespace BackupMonitorClient
{
    public partial class MainWindow : Window
    {
        private ClientController clientController;

        // Entry point of the program
        public MainWindow()
        {
            InitializeComponent();
            clientController = new ClientController();
        }

        // This method is called by clicking on the button "connect to the server"
        private async void Connection(object sender, RoutedEventArgs e)
        {
            await clientController.ConnectToServerAsync();
        }

        // This method is called by clicking on the button "Pause"
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            clientController.SendCommand("PAUSE");
        }

        // This method is called by clicking on the button "Resume"
        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            clientController.SendCommand("RESUME");
        }

        // This method is called by clicking on the button "Stop"
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            clientController.SendCommand("STOP");
        }

        // // This method is called by clicking on the button "Exit"
        private async void Exit(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => clientController.Disconnect());
            Application.Current.Shutdown(); // Close the interface
        }
    }
}