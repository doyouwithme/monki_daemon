using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monki_okpos_daemon.model.master
{
    public class MasterInfoModel
    {
        [JsonProperty("COMMAND")]
        public string COMMAND
        {
            get { return _COMMAND; }
            set { _COMMAND = value; }
        }
        string _COMMAND;

        [JsonProperty("DIRECTION")]
        public string DIRECTION
        {
            get { return _DIRECTION; }
            set { _DIRECTION = value; }
        }
        string _DIRECTION;

        [JsonProperty("EXTERNAL_CODE")]
        public string EXTERNAL_CODE
        {
            get { return _EXTERNAL_CODE; }
            set { _EXTERNAL_CODE = value; }
        }
        string _EXTERNAL_CODE;

        [JsonProperty("RESULT_CODE")]
        public string RESULT_CODE
        {
            get { return _RESULT_CODE; }
            set { _RESULT_CODE = value; }
        }
        string _RESULT_CODE;

        [JsonProperty("RESULT_MSG")]
        public string RESULT_MSG
        {
            get { return _RESULT_MSG; }
            set { _RESULT_MSG = value; }
        }
        string _RESULT_MSG;

        [JsonProperty("SHOP_CD")]
        public string SHOP_CD
        {
            get { return _SHOP_CD; }
            set { _SHOP_CD = value; }
        }
        string _SHOP_CD;

        [JsonProperty("CURRENT_PAGE_NUMBER")]
        public string CURRENT_PAGE_NUMBER
        {
            get { return _CURRENT_PAGE_NUMBER; }
            set { _CURRENT_PAGE_NUMBER = value; }
        }
        string _CURRENT_PAGE_NUMBER;

        [JsonProperty("LAST_DATE")]
        public string LAST_DATE
        {
            get { return _LAST_DATE; }
            set { _LAST_DATE = value; }
        }
        string _LAST_DATE;

        [JsonProperty("TARGET")]
        public string TARGET
        {
            get { return _TARGET; }
            set { _TARGET = value; }
        }
        string _TARGET;

        [JsonProperty("COUNT_PER_PAGE")]
        public string COUNT_PER_PAGE
        {
            get { return _COUNT_PER_PAGE; }
            set { _COUNT_PER_PAGE = value; }
        }
        string _COUNT_PER_PAGE;

        [JsonProperty("TOTAL_PAGE_COUNT")]
        public string TOTAL_PAGE_COUNT
        {
            get { return _TOTAL_PAGE_COUNT; }
            set { _TOTAL_PAGE_COUNT = value; }
        }
        string _TOTAL_PAGE_COUNT;

        [JsonProperty("DATA")]
        public DataModel DATA
        {
            get { return _DATA; }
            set { _DATA = value; }
        }
        DataModel _DATA;
    }
}
