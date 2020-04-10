using Ngrok.Managing.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ngrok.Managing.Forwarding
{
    public class MCNgrokBackgroundWorker : INgrokBackgroundWorker
    {
        public int Interval { get; set; }

        private bool _isStarted = false;
        private Thread _bgThread = null;

        private INgrokTableHelper _tableHelper = null;
        private NgrokServerManager _manager = null;

        public event LogEventHandler LogReceived;

        public MCNgrokBackgroundWorker(INgrokTableHelper tableHelper, NgrokServerManager manager) : this(tableHelper, manager, 60000) { }

        public MCNgrokBackgroundWorker(INgrokTableHelper tableHelper, NgrokServerManager manager, int interval)
        {
            _tableHelper = tableHelper;
            _manager = manager;
            Interval = interval;
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                _bgThread = new Thread(BackgroundActivity);
                _bgThread.Start();
            }
            else
                throw new Exception("BackgroundWorker already started.");
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _isStarted = false;

                while (_bgThread.IsAlive) { };
                _bgThread = null;
            }
            else
                throw new Exception("BackgroundWorker already stopped.");
        }

        public void DoSingleCycle() => BackgroundWork();

        private void BackgroundActivity()
        {
            while (_isStarted)
            {
                BackgroundWork();

                Thread.Sleep(Interval);
            }
        }

        private void BackgroundWork()
        {
            var activeTunnels = _manager.GetTunnels();


        }

        private void Log(string data) => LogReceived?.Invoke(this, new LogEventArgs(data));
    }

    public class LogEventArgs : EventArgs
    {
        public string Data { get; set; }

        public LogEventArgs() : this(string.Empty) { }
        public LogEventArgs(string data) => Data = data;
    }

    public delegate void LogEventHandler(object sender, LogEventArgs e);
}
