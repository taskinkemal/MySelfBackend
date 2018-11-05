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
        public string WebClientAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsProduction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SerilogUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static int MaxRows = 500;


        /// <summary>
        /// 
        /// </summary>
        public class SMTP
        {
            /// <summary>
            /// 
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool EnableSSL { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public class Credentials
            {
                /// <summary>
                /// 
                /// </summary>
                public static string User { get; set; }

                /// <summary>
                /// 
                /// </summary>
                public static string Password { get; set; }
            }
        }
    }
}