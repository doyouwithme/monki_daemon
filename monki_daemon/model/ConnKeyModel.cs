using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model
{
    public class ConnKeyModel
    {
        [JsonProperty("connkey")]
        public string connkey
        {
            get { return _connkey; }
            set { _connkey = value; }
        }
        string _connkey;

        [JsonProperty("shopcode")]
        public string shopcode
        {
            get { return _shopcode; }
            set { _shopcode = value; }
        }
        string _shopcode;
    }
}
