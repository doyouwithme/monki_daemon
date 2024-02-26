namespace monki_okpos_daemon.config
{
    class Config
    {
        public static string RootDirectory = @"C:\MonthlyKitchenPOS";

        public static string API_URL { get; set; }
        public static string MQTT_URL { get; set; }
        public static string Token { get; set; }
        public static string StoreNo { get; set; }
        public static string StoreNm { get; set; }
        public static string EnvironmentMode { get; set; }
        public static string PosInterfaceType { get; set; }
    }


    public class POS_MODEL_TYPES
    {
        public const string OKPOS = "OKPOS";
        public const string METACITYPOS = "METACITYPOS";
        public const string UNIONPOS = "UNIONPOS";
        public const string POSBANK = "POSBANK";
        public const string ASTEMS = "ASTEMS";
    }


}
