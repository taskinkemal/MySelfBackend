using Common.Models.Entities;

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
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Picture url
        /// </summary>
        public string PictureUrl { get; set; }

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

        public User ToUser()
        {
            return new User
            {
                Email = this.Email,
                FirstName = this.FirstName,
                LastName = this.LastName,
                PictureUrl = this.PictureUrl
            };
        }
    }
}
