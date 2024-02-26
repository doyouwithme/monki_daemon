using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.menu
{
    public class SDSCLCODE_MODEL
    {
        //사이드메뉴-선택그룹코드
        [JsonProperty("SDS_GRP_CD")]
        public string SDS_GRP_CD
        {
            get { return _SDS_GRP_CD; }
            set { _SDS_GRP_CD = value; }
        }
        string _SDS_GRP_CD;

        //사이드메뉴-선택분류코드
        [JsonProperty("SDS_CLS_CD")]
        public string SDS_CLS_CD
        {
            get { return _SDS_CLS_CD; }
            set { _SDS_CLS_CD = value; }
        }
        string _SDS_CLS_CD;

        //사이드메뉴-선택분류명
        [JsonProperty("SDS_CLS_NM")]
        public string SDS_CLS_NM
        {
            get { return _SDS_CLS_NM; }
            set { _SDS_CLS_NM = value; }
        }
        string _SDS_CLS_NM;

        //사이드메뉴-선택순서
        [JsonProperty("SDS_ORDER_SEQ")]
        public string SDS_ORDER_SEQ
        {
            get { return _SDS_ORDER_SEQ; }
            set { _SDS_ORDER_SEQ = value; }
        }
        string _SDS_ORDER_SEQ;

        //사이드메뉴-선택수량
        [JsonProperty("SDS_MAX_QTY")]
        public string SDS_MAX_QTY
        {
            get { return _SDS_MAX_QTY; }
            set { _SDS_MAX_QTY = value; }
        }
        string _SDS_MAX_QTY;

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

        //사이드메뉴-KIOSK 필수여부
        [JsonProperty("KIOSK_USE_FG")]
        public string KIOSK_USE_FG
        {
            get { return _KIOSK_USE_FG; }
            set { _KIOSK_USE_FG = value; }
        }
        string _KIOSK_USE_FG;
    }
}
