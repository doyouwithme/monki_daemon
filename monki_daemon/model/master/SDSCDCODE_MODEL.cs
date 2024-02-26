using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.menu
{
    public class SDSCDCODE_MODEL
    {
        //사이드메뉴-선택분류코드
        [JsonProperty("SDS_CLS_CD")]
        public string SDS_CLS_CD
        {
            get { return _SDS_CLS_CD; }
            set { _SDS_CLS_CD = value; }
        }
        string _SDS_CLS_CD;

        //사이드메뉴-선택코드
        [JsonProperty("SDS_CD")]
        public string SDS_CD
        {
            get { return _SDS_CD; }
            set { _SDS_CD = value; }
        }
        string _SDS_CD;

        //사이드메뉴-선택명
        [JsonProperty("SDS_NM")]
        public string SDS_NM
        {
            get { return _SDS_NM; }
            set { _SDS_NM = value; }
        }
        string _SDS_NM;

        //상품코드
        [JsonProperty("PROD_CD")]
        public string PROD_CD
        {
            get { return _PROD_CD; }
            set { _PROD_CD = value; }
        }
        string _PROD_CD;

        //선택메뉴-단가
        [JsonProperty("SDS_PROD_UPRC")]
        public string SDS_PROD_UPRC
        {
            get { return _SDS_PROD_UPRC; }
            set { _SDS_PROD_UPRC = value; }
        }
        string _SDS_PROD_UPRC;

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
