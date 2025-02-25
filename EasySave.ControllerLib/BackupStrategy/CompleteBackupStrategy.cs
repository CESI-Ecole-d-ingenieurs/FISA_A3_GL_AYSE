using EasySave.IviewLib;
using EasySave.ModelLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave.ControllerLib.BackupStrategy
{
    internal class CompleteBackupStrategy : BaseBackupStrategy
    {
        private volatile int networkLoad = 50;
        private CancellationTokenSource _networkMonitorCancellation = new CancellationTokenSource();

        public CompleteBackupStrategy(IBackupView backupview) : base(backupview)
        {
            ShowAvailableNetworkInterfaces();

            Task.Run(() =>
            {
                while (!_networkMonitorCancellation.Token.IsCancellationRequested)
                {
                    networkLoad = GetNetworkLoad();
                    Thread.Sleep(2000);
                }
            }, _networkMonitorCancellation.Token);
        }

        public override async Task ExecuteBackup(string source, string target, String nameBackup, Dictionary<string, bool> _isPaused = null, Dictionary<string, CancellationTokenSource> _cancellationTokens = null)
        {
            DirectoryExist(target);
            var state = BackupStateJournal.ComputeState("CompleteBackup", source, target);

            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var targetDirectory = directory.Replace(source, target);
                DirectoryExist(targetDirectory);
            }

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);

            List<Task> tasks = new List<Task>();
            foreach (var file in files)
            {
                while (networkLoad > 80)
                {
                    Console.WriteLine("Réduction des tâches en parallèle à cause de la charge réseau...");
                    await Task.Delay(2000);
                }

                CancellationToken token = _cancellationTokens[nameBackup].Token;
                if (_cancellationTokens[nameBackup].Token.IsCancellationRequested)
                {
                    state.State = "STOPPED";
                    BackupStateJournal.UpdateState(state);
                    return;
                }

                while (_isPaused[nameBackup])
                {
                    await Task.Delay(500);
                }

                FileInfo fileInfo = new FileInfo(file);
                if ((fileInfo.Length / 1024.0) > GlobalVariables.maximumSize)
                {
                    var task = ProcessLargeFileAsync(source, target, nameBackup, file, token);
                    tasks.Add(task);
                }
                else
                {
                    var task = ProcessSmallFileAsync(source, target, nameBackup, file, token);
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
            _networkMonitorCancellation.Cancel(); // Arrête la surveillance réseau après la sauvegarde
        }

        private int GetNetworkLoad()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                if (interfaces.Length == 0) return 50;

                long totalBytesSentBefore = interfaces.Sum(i => i.GetIPv4Statistics().BytesSent);
                long totalBytesReceivedBefore = interfaces.Sum(i => i.GetIPv4Statistics().BytesReceived);

                Thread.Sleep(1000);

                long totalBytesSentAfter = interfaces.Sum(i => i.GetIPv4Statistics().BytesSent);
                long totalBytesReceivedAfter = interfaces.Sum(i => i.GetIPv4Statistics().BytesReceived);

                long bytesSentPerSec = (totalBytesSentAfter - totalBytesSentBefore);
                long bytesReceivedPerSec = (totalBytesReceivedAfter - totalBytesReceivedBefore);

                float totalUsage = (bytesSentPerSec + bytesReceivedPerSec) * 8 / 1000000;
                if (totalUsage < 0.1f) totalUsage = 0.1f;

                System.Diagnostics.Debug.WriteLine($"📤 Octets envoyés/sec : {bytesSentPerSec}");
                System.Diagnostics.Debug.WriteLine($"📥 Octets reçus/sec : {bytesReceivedPerSec}");
                System.Diagnostics.Debug.WriteLine($"✅ Charge réseau calculée : {totalUsage} Mbps");

                return (int)Math.Min(Math.Max(totalUsage, 0), 100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Erreur lors de la récupération de la charge réseau avec NetworkInterface.");
                System.Diagnostics.Debug.WriteLine($"🔹 Détails : {ex.Message}");
                return 50;
            }
        }

        private void ShowAvailableNetworkInterfaces()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            System.Diagnostics.Debug.WriteLine("🔍 Interfaces réseau détectées :");
            foreach (var iface in interfaces)
            {
                System.Diagnostics.Debug.WriteLine($"- {iface.Name} ({iface.Description})");
            }
        }
    }
}
