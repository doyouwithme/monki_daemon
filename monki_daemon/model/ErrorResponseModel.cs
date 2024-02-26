using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model
{
    public class ErrorResponseModel
    {
        [JsonProperty("message")]
        public string message
        {
            get { return _message; }
            set { _message = value; }
        }
        string _message;

        [JsonProperty("error")]
        public string error
        {
            get { return _error; }
            set { _error = value; }
        }
        string _error;

        [JsonProperty("statusCode")]
        public string statusCode
        {
            get { return _statusCode; }
            set { _statusCode = value;}
        }
        string _statusCode;
    }
}
