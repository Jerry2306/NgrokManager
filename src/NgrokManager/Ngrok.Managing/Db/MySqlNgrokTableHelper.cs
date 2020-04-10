using Ngrok.Managing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ngrok.Managing.Db
{
    public class MySqlNgrokTableHelper : INgrokTableHelper
    {
        private DatabaseManager _manager;
        public MySqlNgrokTableHelper(string cs)
        {
            _manager = new DatabaseManager(cs);
        }

        public int SetForwardAddress(string connectionName, string address)
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName");

            _manager.Open();
            int i = _manager.RunNonQuery($"UPDATE ngrok SET ConnectionAddress = '{address}', ConnectionDate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE ConnectionName = '{connectionName}'");
            _manager.Close();

            return i;
        }

        public ForwardTableEntity GetForwardEntity(string connectionName)
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName");

            var queryResult = _manager.RunQuery($"SELECT ConnectionName,ConnectionAddress,ConnectionDate FROM ngrok WHERE ConnectionName = '{connectionName}'", 3).FirstOrDefault();

            var entity = new ForwardTableEntity(queryResult[0].ToString(), queryResult[1].ToString(), DateTime.Parse(queryResult[2].ToString()));
            return entity;
        }
    }
}
