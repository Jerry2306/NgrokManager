using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NgrokManager.Helper
{
    public class NgrokTableHelper
    {
        private const string McServerForward = "mc_server_forward";
        private DatabaseManager _manager;
        public NgrokTableHelper(string cs)
        {
            _manager = new DatabaseManager(cs);
        }

        public void SetMcForwardAddress(string address)
        {
            int i = _manager.RunNonQuery($"UPDATE ngrok SET ConnectionAddress = '{address}', ConnectionDate = '{DateTime.Now.ToString("yyyy-mm-dd hh:MM:ss")}' WHERE ConnectionName = '{McServerForward}'");
            if (i <= 0)
                throw new Exception($"Es wurde keine Zeile aktualisiert (Beim Setzen der ConnectionAddress). Fehlt evtl. der '{McServerForward}' Eintrag?");
        }
    }
}
