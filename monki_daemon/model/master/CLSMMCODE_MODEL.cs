using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model.master
{
    public class CLSMMCODE_MODEL
    {
        //대분류코드
        [JsonProperty("LCLS_CD")]
        public string LCLS_CD
        {
            get { return _LCLS_CD; }
            set { _LCLS_CD = value; }
        }
        string _LCLS_CD;

        //중분류코드
        [JsonProperty("MCLS_CD")]
        public string MCLS_CD
        {
            get { return _MCLS_CD; }
            set { _MCLS_CD = value; }
        }
        string _MCLS_CD;

        //중분류명
        [JsonProperty("MCLS_NM")]
        public string MCLS_NM
        {
            get { return _MCLS_NM; }
            set { _MCLS_NM = value; }
        }
        string _MCLS_NM;

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
