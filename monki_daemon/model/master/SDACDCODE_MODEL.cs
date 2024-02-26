using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.menu
{
    public class SDACDCODE_MODEL
    {
        //사이드메뉴-속성분류코드
        [JsonProperty("SDA_CLS_CD")]
        public string SDA_CLS_CD
        {
            get { return _SDA_CLS_CD; }
            set { _SDA_CLS_CD = value; }
        }
        string _SDA_CLS_CD;

        //사이드메뉴-속성코드
        [JsonProperty("SDA_CD")]
        public string SDA_CD
        {
            get { return _SDA_CD; }
            set { _SDA_CD = value; }
        }
        string _SDA_CD;

        //사이드메뉴-속성명
        [JsonProperty("SDA_NM")]
        public string SDA_NM
        {
            get { return _SDA_NM; }
            set { _SDA_NM = value; }
        }
        string _SDA_NM;

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
