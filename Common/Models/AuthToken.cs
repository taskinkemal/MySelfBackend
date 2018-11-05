using System;
using System.Runtime.Serialization;


namespace Common.Models
{
    /// <summary>
    /// Authentication token
    /// </summary>
    [DataContract]
    public class AuthToken
    {
        private readonly int userID;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="validUntil"></param>
        public AuthToken(string token, DateTime validUntil)
        {
            Token = token;
            ValidUntil = validUntil;
            DecryptToken(token, out userID, out validUntil);
        }

        /// <summary>
        /// Token string for authentication.
        /// </summary>
        [DataMember]
        public string Token { get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime ValidUntil { get; }

        /// <summary>
        /// 
        /// </summary>
        public int UserID => userID;

        public const string DateTimeFormat = "yyyy-mm-dd";

        private static void DecryptToken(string token, out int userID, out DateTime validUntil)
        {
            userID = -1;
            validUntil = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(token))
            {
                var decryptedToken = new SimpleAES().DecryptString(token);

                var parts = decryptedToken.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    userID = Convertor.ToInt32(parts[0], -1);
                    validUntil = Convertor.ToDateTime(parts[2], DateTime.MinValue, DateTimeFormat);
                }
            }
        }
    }
}
