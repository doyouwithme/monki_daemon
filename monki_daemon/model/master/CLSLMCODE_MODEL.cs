using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.master
{
    public class CLSLMCODE_MODEL
    {
        //대분류코드
        [JsonProperty("LCLS_CD")]
        public string LCLS_CD
        {
            get { return _LCLS_CD; }
            set { _LCLS_CD = value; }
        }
        string _LCLS_CD;

        //대분류명
        [JsonProperty("LCLS_NM")]
        public string LCLS_NM
        {
            get { return _LCLS_NM; }
            set { _LCLS_NM = value; }
        }
        string _LCLS_NM;
                
        //사용여부
        [JsonProperty("USE_YN")]
        public string USE_YN
        {
            get { return _USE_YN; }
            set { _USE_YN = value; }
        }
        string _USE_YN;

        [JsonProperty("INS_DT")]
        public string INS_DT
        {
            get { return _INS_DT; }
            set { _INS_DT = value; }
        }
        string _INS_DT;

        [JsonProperty("UPD_DT")]
        public string UPD_DT
        {
            get { return _UPD_DT; }
            set { _UPD_DT = value; }
        }
        string _UPD_DT;
    }
}
