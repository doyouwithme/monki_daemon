using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model
{
    public class mqttModel
    {
        public string msgid
        {
            get { return _msgid; }
            set { _msgid = value; }
        }
        string _msgid;

        public string data
        {
            get { return _data; }
            set { _data = value; }
        }
        string _data;

        public long timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
        long _timestamp;

    }
}
