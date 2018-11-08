namespace Common.Models
{
    /// <summary>
    /// Wrapper class for user authentication flow.
    /// </summary>
    public class TokenRefreshRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Unique device id.
        /// </summary>
        public string DeviceID { get; set; }
    }
}
