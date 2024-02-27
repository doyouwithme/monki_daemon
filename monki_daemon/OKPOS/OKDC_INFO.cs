using MonkiDaemon.config;
using System;
using System.IO;

namespace MonkiDaemon.OKPOS
{
    class OKDC_INFO
    {
        public static string INI_FILE = Path.Combine(Environment.CurrentDirectory, "monki.ini");
        public const int BUFFER_SIZE = 4096 * 4096;
        public static bool IsRegisterCallback = false;
        public static bool IsFirstConnect = false;
        public static bool CONNECT = false;
        public static string EXTERNAL_CODE = "030";
        public static string EXTERNAL_KEY = "";
        public static string CONN_KEY = "";
        public static string SHOP_CD = "";
        public static string LAST_DATE = "";
        public static int TIMEOUT = 5;
    }

    public class DIRECTION
    {
        public const string POS_TO_EXT = "POS_TO_EXT";
        public const string EXT_TO_POS = "EXT_TO_POS";
    }


    public class OKDC_STATUS
    {
        public const string START = "started";
        public const string FINISH = "finished";
    }

    public class TARGET
    {
        //메뉴
        public const string PRODS = "PRODS";
        //속성분류
        public const string SDACLCODE = "SDACLCODE";
        //속성코드
        public const string SDACDCODE = "SDACDCODE";
        //선택그룹
        public const string SDSGRCODE = "SDSGRCODE";
        //선택분류
        public const string SDSCLCODE = "SDSCLCODE";
        //선택코드
        public const string SDSCDCODE = "SDSCDCODE";
        //테이블
        public const string TABLE = "TABLE";
        //상품분류-대분류
        public const string CLSLMCODE = "CLSLMCODE";
        //상품분류-중분류
        public const string CLSMMCODE = "CLSMMCODE";
        //상품분류-소분류
        public const string CLSSMCODE = "CLSSMCODE";
        //터치키 분류
        public const string TUCLSCODE = "TUCLSCODE";
        //터치상품
        public const string TUKEYCODE = "TUKEYCODE";
        //터치상품+상품정보
        public const string TUKEYCODE_WITH_PROD = "TUKEYCODE_WITH_PROD";
    }

    public class ERROR_CODE
    {
        public const string ERR_0000 = "서버 접속 실패";
        public const string ERR_1001 = "서버 접속 실패";
        public const string ERR_1002 = "서버 접속 실패";
        public const string ERR_1003 = "사용자ID 입력 오류";
        public const string ERR_1004 = "Callback 함수 NULL 오류";
        public const string ERR_1005 = "요청 전문 파싱 오류";
    }

    public class API_LIST
    {
        //conn key
        public static string POST_CONN_KEY = string.Format("/tableorders/pos/getPosConnKey/{0}", Config.StoreNo);
        //store info
        public static string POST_STORE_INFO = string.Format("/tableorders/pos/getStoreInfo/{0}", Config.StoreNo);
        //master info
        public static string POST_MASTER_INFO = string.Format("/tableorders/pos/getMasterInfo/{0}", Config.StoreNo);
        //soldout
        public static string POST_SOLDOUT_CHANGE = string.Format("/tableorders/pos/soldOut/{0}", Config.StoreNo);
        //table move
        public static string POST_TABLE_CHANGE = string.Format("/tableorders/pos/tableChangeMove/{0}", Config.StoreNo);
        //pay completed
        public static string POST_SALE_PARTIAL = string.Format("/tableorders/pos/orderSalePartial/{0}", Config.StoreNo);
        //bill completed
        public static string POST_SALE_ALL = string.Format("/tableorders/pos/orderSaleAll/{0}", Config.StoreNo);
        //cancel
        public static string POST_SALE_CANCEL = string.Format("/tableorders/pos/orderSaleCancle/{0}", Config.StoreNo);
        //order change || table replace/merge
        public static string POST_ORDER_CHANGE = string.Format("/tableorders/pos/orderChange/{0}", Config.StoreNo);
        //req policy
        public static string POST_REQ_POLICY = string.Format("/tableorders/pos/reqPolicy/{0}", Config.StoreNo);
        //return all
        public static string POST_RETURN_ALL = string.Format("/tableorders/pos/orderReturnAll/{0}", Config.StoreNo);
    }
}
