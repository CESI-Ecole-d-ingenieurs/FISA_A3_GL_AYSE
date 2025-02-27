using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProjetWPF;

namespace BackupServer
{

    /// Manages a TCP server that listens for client connections and controls backups remotely.
    public class ServerController
    {

        private readonly MainWindow _mainWindow; // Reference to the main application window
        private Socket _serverSocket; // Server socket for listening to client connections
        private Socket _clientSocket; // Client socket for communication
        private bool _isRunning = true; // Flag to control server execution
        private bool _isPaused = false; // Tracks whether backups are paused
        private CancellationTokenSource _cancellationTokenSource; // Token source for handling cancellations

        /// Initializes the ServerController with a reference to the main window.
        public ServerController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// Starts the server asynchronously and listens for incoming client connections.

        public async Task StartServerAsync()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1200); // Server listens on port 1200

            try
            {
                _serverSocket.Bind(localEndPoint); // Bind the server to the local network
                _serverSocket.Listen(5); // Allow up to 5 pending connections
                UpdateUI("Serveur en attente de connexion...");
                // Accept incoming client connections in a loop
                while (_isRunning)
                {
                    _clientSocket = await Task.Run(() => _serverSocket.Accept()); // Accept client connection
                    UpdateUI("Client connecté !");
                    _ = ListenToClientAsync(); // Start listening to customer orders
                }
            }
            catch (Exception e)
            {
                UpdateUI($"Erreur serveur : {e.Message}");
            }
        }

        /// Asynchronously listens for client commands and processes them.
        private async Task ListenToClientAsync()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            await Task.Run(() =>
            {
                try
                {
                    while (_isRunning && _clientSocket.Connected)
                    {
                        byte[] buffer = new byte[1024]; // Buffer for receiving data
                        int bytesRead = _clientSocket.Receive(buffer); // Receive message from client
                        if (bytesRead == 0) break; // If no data received, exit

                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead); // Convert to string
                        UpdateUI($"Commande reçue : {message}");

                        switch (message.ToUpper())
                        {
                            case "PAUSE":
                                mainWindow.PauseSelectedBackups(null, null);
                                UpdateUI("Sauvegarde en pause...");
                                SendToClient("CONFIRM:PAUSE");
                                break;
                            case "RESUME":
                                mainWindow.ResumeSelectedBackups(null, null);
                                UpdateUI("Reprise de la sauvegarde...");
                                SendToClient("CONFIRM:RESUME");
                                break;
                            case "STOP":
                                mainWindow.StopSelectedBackups(null, null);
                                UpdateUI("Sauvegarde arrêtée.");
                                SendToClient("CONFIRM:STOP");
                                break;
                        }
                    }
                }
                catch (SocketException)
                {
                    UpdateUI("Le client s'est déconnecté.");
                }
                finally
                {
                    _clientSocket?.Close(); // Close the client socket after disconnection
                }
            });
        }

        /// Sends a message to the connected client.
        public void SendToClient(string message)
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _clientSocket.Send(data);
            }
        }

        /// Updates the server status message in the UI and sends it to the client.
        private void UpdateUI(string message)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            Application.Current.Dispatcher.Invoke(() =>
            {
                _mainWindow.ServerStatus.Content = message; // Update UI label with status message
                SendToClient(message); // Send the message to the connected client
            });
        }
    }
}