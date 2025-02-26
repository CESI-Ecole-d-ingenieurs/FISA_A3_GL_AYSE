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
    public class ServerController
    {
        private Socket _serverSocket;
        private Socket _clientSocket;
        private bool _isRunning = true;
        private bool _isPaused = false;
        private CancellationTokenSource _cancellationTokenSource;

        public async Task StartServerAsync()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1200);

            try
            {
                _serverSocket.Bind(localEndPoint);
                _serverSocket.Listen(5);
                UpdateUI("✅ Serveur en attente de connexion...");

                while (_isRunning)
                {
                    _clientSocket = await Task.Run(() => _serverSocket.Accept());
                    UpdateUI("🟢 Client connecté !");
                    _ = ListenToClientAsync(); // 🔹 Lance l'écoute des commandes client
                }
            }
            catch (Exception e)
            {
                UpdateUI($"❌ Erreur serveur : {e.Message}");
            }
        }

        private async Task ListenToClientAsync()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            await Task.Run(() =>
            {
                try
                {
                    while (_isRunning && _clientSocket.Connected)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = _clientSocket.Receive(buffer);
                        if (bytesRead == 0) break;

                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        UpdateUI($"📥 Commande reçue : {message}");

                        switch (message.ToUpper())
                        {
                            case "PAUSE":
                                mainWindow.PauseSelectedBackups(null, null);
                                UpdateUI("⏸ Sauvegarde en pause...");
                                SendToClient("CONFIRM:PAUSE");
                                break;
                            case "RESUME":
                                mainWindow.ResumeSelectedBackups(null, null);
                                UpdateUI("▶️ Reprise de la sauvegarde...");
                                SendToClient("CONFIRM:RESUME");
                                break;
                            case "STOP":
                                mainWindow.StopSelectedBackups(null, null);
                                UpdateUI("⏹ Sauvegarde arrêtée.");
                                SendToClient("CONFIRM:STOP");
                                break;
                        }
                    }
                }
                catch (SocketException)
                {
                    UpdateUI("⚠️ Le client s'est déconnecté.");
                }
                finally
                {
                    _clientSocket?.Close();
                }
            });
        }



        //public async Task StartBackupAsync()
        //{
        //    _cancellationTokenSource = new CancellationTokenSource();
        //    var token = _cancellationTokenSource.Token;
        //    int progress = 0;

        //    while (progress <= 100)
        //    {
        //        if (_isPaused)
        //        {
        //            UpdateUI("⏸ Sauvegarde en pause...");
        //            await Task.Delay(500);
        //            continue;
        //        }

        //        if (token.IsCancellationRequested)
        //        {
        //            UpdateUI("⏹ Sauvegarde arrêtée.");
        //            SendToClient("STOPPED");
        //            return;
        //        }

        //        progress += 5;
        //        UpdateUI($"📊 Sauvegarde en cours... {progress}%");
        //        SendToClient($"PROGRESS:{progress}");

        //        await Task.Delay(1000);
        //    }

        //    UpdateUI("✅ Sauvegarde terminée !");
        //    SendToClient("COMPLETED");
        //}

        public void SendToClient(string message)
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _clientSocket.Send(data);
            }
        }

        private void UpdateUI(string message)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            Application.Current.Dispatcher.Invoke(() =>
            {
                mainWindow.ServerStatus.Content = message;
                SendToClient(message);
            });
        }

        //public void StopServer()
        //{
        //    _clientSocket?.Close();
        //    _serverSocket?.Close();
        //    UpdateUI("🔴 Serveur arrêté.");
        //}

    }
}
