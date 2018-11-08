using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Common;
using Common.Interfaces;
using Common.Models;
using DataLayer.Context;
using DataLayer.Interfaces;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace DataLayer.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthManager : ManagerBase, IAuthManager
    {
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz";
        private readonly ILogManager logManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public AuthManager(MyselfContext context, ILogManager logManager) : base(context)
        {
            this.logManager = logManager;
        }

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
            if (await VerifyFacebookAccessToken(email, accessToken))
            {
                return await GetUserIdFromEmail(email);
            }

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> VerifyGoogleUserAsync(string email, string accessToken)
        {
            if (await VerifyGoogleAccessToken(email, accessToken))
            {
                return await GetUserIdFromEmail(email);
            }

            return -1;
        }

        private async System.Threading.Tasks.Task<int> GetUserIdFromEmail(string email)
        {
            var list = await Context.Users.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).ToListAsync().ConfigureAwait(false);
            
            if (list.Count > 0)
            {
                return list[0].Id;
            }

            var userId = await InsertUserAsync(email);

            return userId;
        }

        private async System.Threading.Tasks.Task<int> InsertUserAsync(string email)
        {
            var user = await Context.Users.AddAsync(new User
            {
                FirstName = "",
                LastName = "",
                Email = email
            });

            await Context.SaveChangesAsync().ConfigureAwait(false);

            return user.Entity.Id;
        }


        private static string GenerateRandomString(int length)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append(Alphabet[rand.Next(Alphabet.Length)]);

            return sb.ToString();
        }

        private async System.Threading.Tasks.Task<bool> VerifyGoogleAccessToken(string email, string accessToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);

            return email.Equals(payload?.Email, StringComparison.OrdinalIgnoreCase);
        }

        private async System.Threading.Tasks.Task<bool> VerifyFacebookAccessToken(string email, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(accessToken))
            {
                return false;
            }

            var facebookGraphUrl = "https://graph.facebook.com/me?fields=id,name,first_name,last_name,email,gender,birthday,picture&access_token=" + accessToken;

            try
            {
                var request = WebRequest.Create(facebookGraphUrl);
                request.Credentials = CredentialCache.DefaultCredentials;

                using (var response = await request.GetResponseAsync())
                {
                    var status = ((HttpWebResponse)response).StatusCode;

                    if (status == HttpStatusCode.OK)
                    {
                        var dataStream = response.GetResponseStream();

                        if (dataStream != null)
                        {
                            var reader = new StreamReader(dataStream);
                            var responseFromServer = reader.ReadToEnd();
                            var facebookUser = JsonConvert.DeserializeObject<FacebookMeResponse>(responseFromServer);

                            return email.Equals(facebookUser?.Email, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                logManager.AddLog(e, "AuthManager.VerifyFacebookAccessToken");
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public class FacebookMeResponse
        {
            /// <summary>
            /// 
            /// </summary>
            public string Email { get; set; }
        }
    }
}
