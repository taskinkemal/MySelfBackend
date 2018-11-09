using System;
using System.Runtime.Serialization;
using Common.Models.Entities;


namespace Common.Models
{
    /// <summary>
    /// Authentication token
    /// </summary>
    [DataContract]
    public class AuthToken
    {
        /// <summary>
        /// Authenticated user id.
        /// </summary>
        public int UserID { get; internal set; }

        /// <summary>
        /// Token string for authentication.
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public static AuthToken FromUserToken(UserToken userToken)
        {
            return new AuthToken
            {
                UserID = userToken.UserId,
                Token = userToken.Token,
                ValidUntil = userToken.ValidUntil
            };
        }
    }
}