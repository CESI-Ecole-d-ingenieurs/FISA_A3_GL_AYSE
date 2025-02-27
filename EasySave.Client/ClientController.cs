using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BackupMonitorClient
{
    /// Manages the client-side communication with the backup server.
    /// Handles connection, sending commands, and receiving status updates.
    public class ClientController
    {

        private Socket _clientSocket; // Client socket for communication


        /// This method create a socket for the client
        /// and connect the client to the server.
        public async Task ConnectToServerAsync()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var infos = (Label)mainWindow.FindName("Informations");

            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    infos.Content = "Connexion au serveur...";
                });

                try
                {
                    IPAddress ipAddress = IPAddress.Loopback; // Localhost address
                    IPEndPoint serverEndpoint = new IPEndPoint(ipAddress, 1200); // Server port 1200
                    _clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // creation of the socket for the client
                    _clientSocket.Connect(serverEndpoint); // connection to the server

                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        infos.Content = "Connecté au serveur.";
                    });

                    // Start listening for messages from the server
                    _ = ListenToServerAsync();
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        mainWindow.Informations.Content = "Erreur : " + e.Message;

                    });
                }
            });
        }

        /// This method retrieves the messages from the server
        private async Task ListenToServerAsync()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            await Task.Run(() =>
            {
                try
                {
                    byte[] buffer = new byte[1024];

                    while (_clientSocket != null && _clientSocket.Connected)
                    {
                        int bytesRead = _clientSocket.Receive(buffer); // Receive the message from the server
                        if (bytesRead == 0) break;

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim(); // Transform the bytes to a string

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Console.WriteLine($"Message reçu du serveur : {message}");
                            
                            if (message.StartsWith("CONFIRM:"))
                            {
                                string command = message.Replace("CONFIRM:", "");

                                switch (command)
                                {
                                    case "PAUSE":

                                        mainWindow.Informations.Content = "Sauvegarde mise en pause par le serveur.";
                                        break;
                                    case "RESUME":
                                        mainWindow.Informations.Content = "Reprise de la sauvegarde par le serveur.";
                                        break;
                                    case "STOP":
                                        mainWindow.Informations.Content = "Sauvegarde arrêtée par le serveur.";

                                        break;
                                }
                            }
                            else if (message.StartsWith("[") && message.Contains("]"))
                            {

                                // Update the progress

                                mainWindow.State_t.Text = string.Join("\n", message.Split('\n').Distinct());
                            }
                        });
                    }
                }
                catch (SocketException)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        mainWindow.Informations.Content = "Déconnecté du serveur.";

                    });
                }
                finally
                {
                    // Close the connection between the client and the server.
                    Disconnect();
                }
            });
        }

        /// This method send a message to the server
        public void SendCommand(string command)
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                try
                {
                    byte[] data = Encoding.ASCII.GetBytes(command); // Transform the string into bytes
                    _clientSocket.Send(data); // Send the bytes to the server
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Erreur d'envoi de la commande '{command}' : {ex.Message}");
                    Disconnect();
                }
            }
            else
            {
                Console.WriteLine("Tentative d'envoi alors que le client est déconnecté.");
            }
        }

        /// This method close the connection between the client and the server.
        public void Disconnect()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (_clientSocket != null && _clientSocket.Connected)
            {
                try
                {
                    SendCommand("DISCONNECT"); // Inform the server about the disconnection
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                    _clientSocket = null;
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (mainWindow != null)
                        {

                            mainWindow.Informations.Content = "Erreur de déconnexion : " + e.Message;

                        }
                    });
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (mainWindow != null)
                {

                    mainWindow.Close(); // Close the interface of the client

                }
            });
        }


    }
}
