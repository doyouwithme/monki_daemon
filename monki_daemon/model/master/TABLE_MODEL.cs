using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model.menu
{
    public class TABLE_MODEL
    {
        //테이블코드
        [JsonProperty("TABLE_CD")]
        public string TABLE_CD
        {
            get { return _TABLE_CD; }
            set { _TABLE_CD = value; }
        }
        string _TABLE_CD;

        //테이블명
        [JsonProperty("TABLE_NM")]
        public string TABLE_NM
        {
            get { return _TABLE_NM; }
            set { _TABLE_NM = value; }
        }
        string _TABLE_NM;

        //테이블그룹코드
        [JsonProperty("TABLE_GR_CD")]
        public string TABLE_GR_CD
        {
            get { return _TABLE_GR_CD; }
            set { _TABLE_GR_CD = value; }
        }
        string _TABLE_GR_CD;

        //테이블그룹이름
        [JsonProperty("TABLE_GR_NM")]
        public string PROD_CD
        {
            get { return _TABLE_GR_NM; }
            set { _TABLE_GR_NM = value; }
        }
        string _TABLE_GR_NM;

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
