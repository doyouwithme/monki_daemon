using monki_okpos_daemon.model.menu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.master
{
    public class DataModel
    {
        //메뉴
        [JsonProperty("PRODS")]
        public List<PRODS_MODEL> PRODS
        {
            get { return _PRODS; }
            set { _PRODS = value; }
        }
        List<PRODS_MODEL> _PRODS;

        //속성분류
        [JsonProperty("SDACLCODE")]
        public List<SDACLCODE_MODEL> SDACLCODE
        {
            get { return _SDACLCODE; }
            set { _SDACLCODE = value; }
        }
        List<SDACLCODE_MODEL> _SDACLCODE;

        //속성코드
        [JsonProperty("SDACDCODE")]
        public List<SDACDCODE_MODEL> SDACDCODE
        {
            get { return _SDACDCODE; }
            set { _SDACDCODE = value; }
        }
        List<SDACDCODE_MODEL> _SDACDCODE;

        //선택그룹
        [JsonProperty("SDSGRCODE")]
        public List<SDSGRCODE_MODEL> SDSGRCODE
        {
            get { return _SDSGRCODE; }
            set { _SDSGRCODE = value; }
        }
        List<SDSGRCODE_MODEL> _SDSGRCODE;

        //선택분류
        [JsonProperty("SDSCLCODE")]
        public List<SDSCLCODE_MODEL> SDSCLCODE
        {
            get { return _SDSCLCODE; }
            set { _SDSCLCODE = value; }
        }
        List<SDSCLCODE_MODEL> _SDSCLCODE;

        //선택코드
        [JsonProperty("SDSCDCODE")]
        public List<SDSCDCODE_MODEL> SDSCDCODE
        {
            get { return _SDSCDCODE; }
            set { _SDSCDCODE = value; }
        }
        List<SDSCDCODE_MODEL> _SDSCDCODE;

        //테이블
        [JsonProperty("TABLE")]
        public List<TABLE_MODEL> TABLE
        {
            get { return _TABLE; }
            set { _TABLE = value; }
        }
        List<TABLE_MODEL> _TABLE;

        //대분류
        [JsonProperty("CLSLMCODE")]
        public List<CLSLMCODE_MODEL> CLSLMCODE
        {
            get { return _CLSLMCODE; }
            set { _CLSLMCODE = value; }
        }
        List<CLSLMCODE_MODEL> _CLSLMCODE;

        //중분류
        [JsonProperty("CLSMMCODE")]
        public List<CLSMMCODE_MODEL> CLSMMCODE
        {
            get { return _CLSMMCODE; }
            set { _CLSMMCODE = value; }
        }
        List<CLSMMCODE_MODEL> _CLSMMCODE;

        //소분류
        [JsonProperty("CLSSMCODE")]
        public List<CLSSMCODE_MODEL> CLSSMCODE
        {
            get { return _CLSSMCODE; }
            set { _CLSSMCODE = value; }
        }
        List<CLSSMCODE_MODEL> _CLSSMCODE;

        // 터치키 분류
        [JsonProperty("TUCLSCODE")]
        public List<TUCLSCODE_MODEL> TUCLSCODE
        {
            get { return _TUCLSCODE; }
            set { _TUCLSCODE = value; }
        }
        List<TUCLSCODE_MODEL> _TUCLSCODE;

        //터치상품
        [JsonProperty("TUKEYCODE")]
        public List<TUKEYCODE_MODEL> TUKEYCODE
        {
            get { return _TUKEYCODE; }
            set { _TUKEYCODE = value; }
        }
        List<TUKEYCODE_MODEL> _TUKEYCODE;

        //터치상품+상품정보
        [JsonProperty("TUKEYCODE_WITH_PROD")]
        public List<TUKEYCODE_WITH_PROD_MODEL> TUKEYCODE_WITH_PROD
        {
            get { return _TUKEYCODE_WITH_PROD; }
            set { _TUKEYCODE_WITH_PROD = value; }
        }
        List<TUKEYCODE_WITH_PROD_MODEL> _TUKEYCODE_WITH_PROD;
    }
}
