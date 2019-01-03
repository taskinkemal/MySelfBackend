namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsProduction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSqLite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static int MaxRows = 500;
    }
}