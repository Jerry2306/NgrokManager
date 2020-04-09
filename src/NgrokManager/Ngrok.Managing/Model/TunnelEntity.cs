using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ngrok.Managing.Model
{
    public class TunnelEntity
    {
        public string name = string.Empty;
        public string uri = string.Empty;
        public string public_url = string.Empty;
        public string proto = string.Empty;
        public Config config = new Config();
    }

    public class Config
    {
        public string addr = string.Empty;
        public string inspect = string.Empty;
    }
}
