using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ngrok.Managing.Db
{
    public interface INgrokTableHelper
    {
        int SetForwardAddress(string connectionName, string address);

        ForwardTableEntity GetForwardEntity(string connectionName);
    }

    public class ForwardTableEntity
    {
        public string ConnectionName { get; set; }
        public string ConnectionAddress { get; set; }
        public DateTime ConnectionDate { get; set; }

        public ForwardTableEntity() : this(string.Empty, string.Empty, DateTime.MinValue) { }

        public ForwardTableEntity(string name, string address, DateTime date)
        {
            ConnectionName = name;
            ConnectionAddress = address;
            ConnectionDate = date;
        }
    }
}
