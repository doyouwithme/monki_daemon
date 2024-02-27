using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.model.master
{
    public class TUKEYCODE_MODEL
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
    }
}
