using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ngrok.Managing.Forwarding
{
    public interface INgrokBackgroundWorker
    {
        int Interval { get; set; }
        event LogEventHandler LogReceived;

        void Start();
        void Stop();

        void DoSingleCycle();
    }
}
