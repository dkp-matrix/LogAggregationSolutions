namespace LokiLogger
{
    public class AppGlobal
    {
        public const string GrafanaUrl = "http://192.168.27.169:3000";
        public const string LokiUrl = "http://192.168.27.169:3100";
        public const string QueryRange_LokiEndpoint = "/loki/api/v1/query_range";
        public const string OutputTemplate = "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}";

        // Customizable test parameters
        public static readonly int TestDurationSeconds = 1;
        public static readonly int[] LogRatesPerSecond = { 10_000 };

        public static string UserName = "Darshan Parmar";
    }
}
