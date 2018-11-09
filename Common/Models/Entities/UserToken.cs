using System;

namespace Common.Models.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ValidUntil { get; set; }
    }
}
