using Ngrok.Managing.Data;
using Ngrok.Managing.Db;
using Ngrok.Managing.Forwarding;
using Ngrok.Managing.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NgrokManager.ViewModel
{
    public class MainWindowViewModel : BasePropertyChanged
    {
        public string MainButtonContent { get; set; }
        public string LabelProgress { get; set; }

        public ObservableCollection<string> BackgroundWorkerLog { get; set; }

        private string _ngrokBatchPath = string.Empty;
        private string _csFilePath = string.Empty;

        private TunnelEntity _tunnel = null;
        private INgrokTableHelper _helper;
        private NgrokServerManager _manager;

        public MainWindowViewModel()
        {
            MainButtonContent = "Start";
            LabelProgress = "...";


            BackgroundWorkerLog = new ObservableCollection<string>();

            _csFilePath = ConfigurationManager.AppSettings["ConnectionStringFilePath"];
            try
            {
                _helper = new MySqlNgrokTableHelper(File.ReadAllText(_csFilePath));
            }
            catch (Exception exc)
            {
                throw new Exception($"ConnectionString Datei konnte nicht gelesen werden! Datei: '{_csFilePath}'", exc);
            }

            _manager = new NgrokServerManager();
            _ngrokBatchPath = ConfigurationManager.AppSettings["HostNgrokBatchPath"];
            StartAPI();
        }

        public void StartAPI()
        {
            Log("Starting API...");
            _manager.StartAPI(_ngrokBatchPath);
            Log("API started!");
        }

        public void StopAPI()
        {
            Log("Stopping API...");
            _manager.StopAPI();
            Log("API stopped!");
        }

        public void StartStop()
        {
            if (MainButtonContent.ToLower() == "start")
            {
                try
                {
                    _tunnel = _manager.StartTunneling(Const.McServerForward, "tcp", "25565");
                }
                catch (Exception exc)
                {
                    throw new Exception($"Tunnel-Fehler: {exc.Message}", exc);
                }

                MainButtonContent = "Stop";

                try
                {
                    _helper.SetForwardAddress(Const.McServerForward, _tunnel.public_url);
                }
                catch (Exception exc)
                {
                    throw new Exception($"Datenbank-Fehler: {exc.Message}", exc);
                }

                Log("Tunnel geöffnet und Datenbankeintrag angepasst!");
            }
            else if (MainButtonContent.ToLower() == "stop")
            {
                try
                {
                    _manager.StopTunneling(Const.McServerForward);
                }
                catch (Exception exc)
                {
                    throw new Exception($"Tunnel-Fehler: {exc.Message}", exc);
                }

                _tunnel = null;
                MainButtonContent = "Start";

                try
                {
                    _helper.SetForwardAddress(Const.McServerForward, string.Empty);
                }
                catch (Exception exc)
                {
                    throw new Exception($"Datenbank-Fehler: {exc.Message}", exc);
                }

                Log("Tunnel geschlossen und Datenbankeintrag angepasst!");
            }
        }

        public void RefreshAddress()
        {
            try
            {
                if (_tunnel != null)
                    _manager.StopTunneling(Const.McServerForward);

                _tunnel = _manager.StartTunneling(Const.McServerForward, "tcp", "25565");
            }
            catch (Exception exc)
            {
                throw new Exception($"Tunnel-Fehler: {exc.Message}", exc);
            }

            try
            {
                _helper.SetForwardAddress(Const.McServerForward, _tunnel.public_url);
            }
            catch (Exception exc)
            {
                throw new Exception($"Datenbank-Fehler: {exc.Message}", exc);
            }

            Log("Tunnel erneuert und Datenbankeintrag angepasst!");
        }

        private void Log(string msg) => BackgroundWorkerLog.Add(msg);
    }
}