namespace Common.Models
{
    /// <summary>
    /// Wrapper class for user authentication flow.
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Facebook token
        /// </summary>
        public string FacebookToken { get; set; }

        /// <summary>
        /// Google token
        /// </summary>
        public string GoogleToken { get; set; }
        
        /// <summary>
        /// Unique device id.
        /// </summary>
        public string DeviceID { get; set; }
    }
}
