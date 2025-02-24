using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackupMonitorClient
{
    public class ClientController
    {
        private readonly MainWindow _mainWindow;
        private Socket _clientSocket;

        public ClientController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public async Task ConnectToServerAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mainWindow.Informations.Content = "Connexion au serveur...";
                });

                try
                {
                    IPAddress ipAddress = IPAddress.Loopback;
                    IPEndPoint serverEndpoint = new IPEndPoint(ipAddress, 1200);
                    _clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    _clientSocket.Connect(serverEndpoint);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _mainWindow.Informations.Content = "✅ Connecté au serveur.";
                    });

                    _ = ListenToServerAsync();
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _mainWindow.Informations.Content = "❌ Erreur : " + e.Message;
                    });
                }
            });
        }

        private async Task ListenToServerAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    byte[] buffer = new byte[1024];

                    while (_clientSocket != null && _clientSocket.Connected)
                    {
                        int bytesRead = _clientSocket.Receive(buffer);
                        if (bytesRead == 0) break;

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim(); // ✅ Utilise UTF-8

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Console.WriteLine($"📥 Message reçu du serveur : {message}");

                            // ✅ Vérifie si c'est une confirmation de commande
                            if (message.StartsWith("CONFIRM:"))
                            {
                                string command = message.Replace("CONFIRM:", ""); // Supprime "CONFIRM:"

                                switch (command)
                                {
                                    case "PAUSE":
                                        _mainWindow.Informations.Content = "⏸️ Sauvegarde mise en pause par le serveur.";
                                        break;
                                    case "RESUME":
                                        _mainWindow.Informations.Content = "▶️ Reprise de la sauvegarde par le serveur.";
                                        break;
                                    case "STOP":
                                        _mainWindow.Informations.Content = "⏹ Sauvegarde arrêtée par le serveur.";
                                        break;
                                }
                            }
                            else if (message.StartsWith("[") && message.Contains("]"))
                            {
                                // ✅ Mise à jour de la progression
                                _mainWindow.State_t.Text = string.Join("\n", message.Split('\n').Distinct());
                            }
                        });
                    }
                }
                catch (SocketException)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _mainWindow.Informations.Content = "⚠️ Déconnecté du serveur.";
                    });
                }
                finally
                {
                    Disconnect();
                }
            });
        }








        public void SendCommand(string command)
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                try
                {
                    byte[] data = Encoding.ASCII.GetBytes(command);
                    _clientSocket.Send(data);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"⚠️ Erreur d'envoi de la commande '{command}' : {ex.Message}");
                    Disconnect();
                }
            }
            else
            {
                Console.WriteLine("⚠️ Tentative d'envoi alors que le client est déconnecté.");
            }
        }



        public void Disconnect()
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                try
                {
                    SendCommand("DISCONNECT"); // 🔹 Informe le serveur avant de fermer
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                    _clientSocket = null;
                }
                catch (Exception e)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_mainWindow != null)
                        {
                            _mainWindow.Informations.Content = "❌ Erreur de déconnexion : " + e.Message;
                        }
                    });
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_mainWindow != null)
                {
                    _mainWindow.Close();
                }
            });
        }


    }
}
