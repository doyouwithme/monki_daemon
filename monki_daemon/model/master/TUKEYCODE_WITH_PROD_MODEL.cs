using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.master
{
    public class TUKEYCODE_WITH_PROD_MODEL
    {
        //터치키구분 (S:터치키, T:서브분류
        [JsonProperty("TU_FG")]
        public string TU_FG
        {
            get { return _TU_FG; }
            set { _TU_FG = value; }
        }
        string _TU_FG;

        //터치분류코드
        [JsonProperty("TU_CLS_CD")]
        public string TU_CLS_CD
        {
            get { return _TU_CLS_CD; }
            set { _TU_CLS_CD = value; }
        }
        string _TU_CLS_CD;

        //터치키코드
        [JsonProperty("TU_KEY_CD")]
        public string TU_KEY_CD
        {
            get { return _TU_KEY_CD; }
            set { _TU_KEY_CD = value; }
        }
        string _TU_KEY_CD;

        //상품코드
        [JsonProperty("PROD_CD")]
        public string PROD_CD
        {
            get { return _PROD_CD; }
            set { _PROD_CD = value; }
        }
        string _PROD_CD;

        //페이지수
        [JsonProperty("TU_PAGE")]
        public string TU_PAGE
        {
            get { return _TU_PAGE; }
            set { _TU_PAGE = value; }
        }
        string _TU_PAGE;

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

        //메뉴명
        [JsonProperty("PROD_NM")]
        public string PROD_NM
        {
            get { return _PROD_NM; }
            set { _PROD_NM = value; }
        }
        string _PROD_NM;

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

        //소분류코드
        [JsonProperty("SCLS_CD")]
        public string SCLS_CD
        {
            get { return _SCLS_CD; }
            set { _SCLS_CD = value; }
        }
        string _SCLS_CD;

        //판매단가
        [JsonProperty("SALE_UPRC")]
        public string SALE_UPRC
        {
            get { return _SALE_UPRC; }
            set { _SALE_UPRC = value; }
        }
        string _SALE_UPRC;

        //과세여부 (Y:과세, N:면세)
        [JsonProperty("TAX_YN")]
        public string TAX_YN
        {
            get { return _TAX_YN; }
            set { _TAX_YN = value; }
        }
        string _TAX_YN;

        //사이드메뉴 사용여부 (Y:사용, N:미사용)
        [JsonProperty("SIDE_MENU_YN")]
        public string SIDE_MENU_YN
        {
            get { return _SIDE_MENU_YN; }
            set { _SIDE_MENU_YN = value; }
        }
        string _SIDE_MENU_YN;

        ///사이드메뉴 속성분류코드
        [JsonProperty("SDA_CLS_CD")]
        public string SDA_CLS_CD
        {
            get { return _SDA_CLS_CD; }
            set { _SDA_CLS_CD = value; }
        }
        string _SDA_CLS_CD;

        //사이드메뉴 선택그룹코드
        [JsonProperty("SDS_GRP_CD")]
        public string SDS_GRP_CD
        {
            get { return _SDS_GRP_CD; }
            set { _SDS_GRP_CD = value; }
        }
        string _SDS_GRP_CD;

        //최소주문수량
        [JsonProperty("ORD_MIN_QTY")]
        public string ORD_MIN_QTY
        {
            get { return _ORD_MIN_QTY; }
            set { _ORD_MIN_QTY = value; }
        }
        string _ORD_MIN_QTY;

        [JsonProperty("USE_YN")]
        public string USE_YN
        {
            get { return _USE_YN; }
            set { _USE_YN = value; }
        }
        string _USE_YN;

        [JsonProperty("SOLD_OUT_YN")]
        public string SOLD_OUT_YN
        {
            get { return _SOLD_OUT_YN; }
            set { _SOLD_OUT_YN = value; }
        }
        string _SOLD_OUT_YN;
    }
}
