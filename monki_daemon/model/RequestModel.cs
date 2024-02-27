using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model
{
    class RequestModel
    {
        [JsonProperty("code")]
        public string code
        {
            get { return _code; }
            set { _code = value; }
        }
        string _code;

        [JsonProperty("message")]
        public string message
        {
            get { return _message; }
            set { _message = value; }
        }
        string _message;
    }

}
