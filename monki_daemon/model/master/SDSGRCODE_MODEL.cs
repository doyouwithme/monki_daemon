using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model.menu
{
    public class SDSGRCODE_MODEL
    {
        //사이드메뉴-선택그룹코드
        [JsonProperty("SDS_GRP_CD")]
        public string SDS_GRP_CD
        {
            get { return _SDS_GRP_CD; }
            set { _SDS_GRP_CD = value; }
        }
        string _SDS_GRP_CD;

        //사이드메뉴-선택그룹명
        [JsonProperty("SDS_GRP_NM")]
        public string SDS_GRP_NM
        {
            get { return _SDS_GRP_NM; }
            set { _SDS_GRP_NM = value; }
        }
        string _SDS_GRP_NM;

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
