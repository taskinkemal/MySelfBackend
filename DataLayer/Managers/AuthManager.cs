using System;
using System.Text;
using Common;
using Common.Models;
using DataLayer.Context;
using DataLayer.Interfaces;


namespace DataLayer.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthManager : ManagerBase, IAuthManager
    {
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public AuthManager(MyselfContext context) : base(context) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="validUntil"></param>
        /// <returns></returns>
        public AuthToken GenerateToken(int userID, DateTime validUntil)
        {
            return new AuthToken(new SimpleAES().EncryptToString(userID + "_" + GenerateRandomString(50) + "_" + validUntil.ToString(AuthToken.DateTimeFormat)), validUntil);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsTokenValid(AuthToken token)
        {
            //TODO: check from DB.
            if (token.UserID > -1 && token.ValidUntil > DateTime.Now)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> VerifyFacebookUserAsync(string email, string accessToken)
        {
            return 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> VerifyGoogleUserAsync(string email, string accessToken)
        {
            return 1;
        }


        private static string GenerateRandomString(int length)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append(Alphabet[rand.Next(Alphabet.Length)]);

            return sb.ToString();
        }
    }
}
