using Ngrok.Managing.Forwarding;
using Ngrok.Managing.Model;
using NgrokManager.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        private TunnelEntity _tunnel = null;
        private NgrokTableHelper _helper;
        private NgrokServerManager _manager;

        public MainWindowViewModel()
        {
            MainButtonContent = "Start";
            LabelProgress = "...";

            _helper = new NgrokTableHelper("Data Source=den1.mysql1.gear.host;Initial Catalog=serverconfig;User ID=serverconfig;Password=Rf0o!p!4hhgz");

            _manager = new NgrokServerManager();
            _ngrokBatchPath = ConfigurationManager.AppSettings["HostNgrokBatchPath"];
            _manager.StartAPI(_ngrokBatchPath);
        }

        public void StartAPI() => _manager.StartAPI(_ngrokBatchPath);

        public void StartStop()
        {
            if (MainButtonContent.ToLower() == "start")
            {
                _tunnel = _manager.StartTunneling("mc_server_forward", "tcp", "25565");
                _helper.SetMcForwardAddress(_tunnel.public_url);
                MainButtonContent = "Stop";
                MessageBox.Show("Tunnel geöffnet!");
            }
            else if (MainButtonContent.ToLower() == "stop")
            {
                _manager.StopTunneling("mc_server_forward");
                _helper.SetMcForwardAddress(string.Empty);
                _tunnel = null;
                MainButtonContent = "Start";
                MessageBox.Show("Tunnel geschlossen!");
            }
        }

        public void RefreshAddress()
        {
            if (_tunnel != null)
                _manager.StopTunneling("mc_server_forward");

            _tunnel = _manager.StartTunneling("mc_server_forward", "tcp", "25565");
            _helper.SetMcForwardAddress(_tunnel.public_url);
            MessageBox.Show("Adresse erneuert!");
        }
    }
}