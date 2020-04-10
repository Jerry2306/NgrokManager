using Newtonsoft.Json;
using Ngrok.Managing.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ngrok.Managing.Forwarding
{
    public class NgrokServerManager
    {
        private HttpWebRequest _request { get; set; }
        private Process _process { get; set; }
        //TODO: Vll noch event für Start/Stop -> Anhand Process, wenn null -> stopped -> sonst started
        public NgrokServerManager()
        {
            _request = null;
            _process = null;
        }

        public void StartAPI(string fileName)
        {
            /*var psi = new ProcessStartInfo(fileName);
            psi.CreateNoWindow = false;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            */
            if (_process != null)
                if (!ContinueAlreadyExistingProcess())
                    return;

            //TODO: Check if already hosting and alert
            //TODO: Implement NgrokBackgroundWorker/Checker
            _process = Process.Start(new ProcessStartInfo(fileName) { WindowStyle = ProcessWindowStyle.Minimized });
            /*
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.BeginOutputReadLine();

            _process.ErrorDataReceived += _process_ErrorDataReceived;
            
            _process.Exited += _process_Exited;*/
        }

        public void StopAPI()
        {
            if (_process == null)
                throw new Exception("Process is null. Shut down process manually.");
            //TODO: find process ngrok.exe and cancel all found processes
            _process.Close();
            _process.Dispose();
            _process = null;
        }

        private int Dummy() => 0;
        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dummy();
        }

        private void _process_Exited(object sender, EventArgs e)
        {
            _process?.Dispose();
            _process = null;
        }

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dummy();
        }

        public TunnelEntity StartTunneling(string name, string proto, string port)
        {
            _request = (HttpWebRequest)WebRequest.Create($"http://localhost:4040/api/tunnels");
            _request.Method = "POST";
            _request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(_request.GetRequestStream()))
            {
                string json = $"{{\"addr\": \"{port}\", \"proto\": \"{proto}\", \"name\": \"{name}\"}}";

                streamWriter.Write(json);
            }

            string result;
            using (var streamReader = new StreamReader(((HttpWebResponse)_request.GetResponse()).GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TunnelEntity>(result);
        }

        public string StopTunneling(string name)
        {
            _request = (HttpWebRequest)WebRequest.Create($"http://localhost:4040/api/tunnels/{name}");
            _request.Method = "DELETE";
            _request.ContentType = "application/json";

            string result;
            using (var streamReader = new StreamReader(((HttpWebResponse)_request.GetResponse()).GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        public TunnelListEntity GetTunnels()
        {
            _request = (HttpWebRequest)WebRequest.Create($"http://localhost:4040/api/tunnels");
            _request.Method = "GET";
            _request.ContentType = "application/json";

            string result;
            using (var streamReader = new StreamReader(((HttpWebResponse)_request.GetResponse()).GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TunnelListEntity>(result);
        }

        private bool ContinueAlreadyExistingProcess()
        {
            if (MessageBox.Show("Ein Prozess ist bereits hinterlegt. Soll der Prozess geschlossen und anschließend fortgefahren werden?", "Achtung", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _process.Close();
                _process.Dispose();
                _process = null;

                return true;
            }

            return false;
        }
    }
}