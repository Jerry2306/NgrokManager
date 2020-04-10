using Ngrok.Managing.Data;
using Ngrok.Managing.Forwarding;
using Ngrok.Managing.Model;
using NgrokManager.Helper;
using System;
using System.Collections.Generic;
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

        private string _ngrokBatchPath = string.Empty;
        private string _csFilePath = string.Empty;

        private TunnelEntity _tunnel = null;
        private NgrokTableHelper _helper;
        private NgrokServerManager _manager;

        public MainWindowViewModel()
        {
            MainButtonContent = "Start";
            LabelProgress = "...";

            _csFilePath = ConfigurationManager.AppSettings["ConnectionStringFilePath"];
            try
            {
                _helper = new NgrokTableHelper(File.ReadAllText(_csFilePath));
            }
            catch (Exception exc)
            {
                throw new Exception($"ConnectionString Datei konnte nicht gelesen werden! Datei: '{_csFilePath}'", exc);
            }

            _manager = new NgrokServerManager();
            _ngrokBatchPath = ConfigurationManager.AppSettings["HostNgrokBatchPath"];
            _manager.StartAPI(_ngrokBatchPath);
        }

        public void StartAPI() => _manager.StartAPI(_ngrokBatchPath);

        public void StopAPI() => _manager.StopAPI();

        public void StartStop()
        {
            if (MainButtonContent.ToLower() == "start")
            {
                _tunnel = _manager.StartTunneling(Const.McServerForward, "tcp", "25565");
                MainButtonContent = "Stop";
                MessageBox.Show("Tunnel geöffnet! Datenbankeintrag wird angepasst...");
                _helper.SetMcForwardAddress(_tunnel.public_url);
                MessageBox.Show("In Datenbank aktualisiert!");
            }
            else if (MainButtonContent.ToLower() == "stop")
            {
                _manager.StopTunneling(Const.McServerForward);
                _tunnel = null;
                MainButtonContent = "Start";
                MessageBox.Show("Tunnel geschlossen! Datenbankeintrag wird angepasst...");
                _helper.SetMcForwardAddress(string.Empty);
                MessageBox.Show("In Datenbank aktualisiert!");
            }
        }

        public void RefreshAddress()
        {
            if (_tunnel != null)
                _manager.StopTunneling(Const.McServerForward);

            _tunnel = _manager.StartTunneling(Const.McServerForward, "tcp", "25565");
            MessageBox.Show("Adresse erneuert! Datenbankeintrag wird angepasst...");
            
            _helper.SetMcForwardAddress(_tunnel.public_url);
            MessageBox.Show("In Datenbank aktualisiert!");
        }
    }
}