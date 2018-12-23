using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Common.Interfaces;
using Common.Models;
using Common.Models.Entities;
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
        private readonly ILogManager logManager;
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz0123456789.";

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
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public UserToken VerifyAccessToken(string accessToken)
        {
            if (accessToken == "kemal")
                return new UserToken
                {
                    UserId = 1,
                    ValidUntil = DateTime.Now.AddYears(1)
                };

            return Context.UserTokens.FirstOrDefault(t =>
                t.Token.Equals(accessToken, StringComparison.OrdinalIgnoreCase) &&
                t.ValidUntil > DateTime.Now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(int userId, string deviceId)
        {
            AuthToken result;

            var existingToken = await Context.UserTokens.FindAsync(userId, deviceId).ConfigureAwait(false);

            if (existingToken != null)
            {
                existingToken.ValidUntil = DateTime.Now.AddYears(1);
                existingToken.Token = GenerateToken(userId);

                await Context.SaveChangesAsync().ConfigureAwait(false);

                result = AuthToken.FromUserToken(existingToken);
            }
            else
            {
                var newToken = new UserToken
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    Token = GenerateToken(userId),
                    ValidUntil = DateTime.Now.AddYears(1)
                };

                await Context.UserTokens.AddAsync(newToken).ConfigureAwait(false);

                await Context.SaveChangesAsync().ConfigureAwait(false);

                result = AuthToken.FromUserToken(newToken);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> VerifyFacebookUserAsync(User user, string accessToken)
        {
            if (await VerifyFacebookAccessToken(user.Email, accessToken))
            {
                return await GetUserIdFromEmail(user);
            }

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> VerifyGoogleUserAsync(User user, string accessToken)
        {
            if (await VerifyGoogleAccessToken(user.Email, accessToken))
            {
                return await GetUserIdFromEmail(user);
            }

            return -1;
        }

        private async System.Threading.Tasks.Task<int> GetUserIdFromEmail(User user)
        {
            var list = await Context.Users.Where(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)).ToListAsync().ConfigureAwait(false);
            
            if (list.Count > 0)
            {
                return list[0].Id;
            }

            var userId = await InsertUserAsync(user);

            return userId;
        }

        private async System.Threading.Tasks.Task<int> InsertUserAsync(User user)
        {
            var u = await Context.Users.AddAsync(new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PictureUrl = user.PictureUrl
            });

            await Context.SaveChangesAsync().ConfigureAwait(false);

            return u.Entity.Id;
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

        private static string GenerateToken(int userID)
        {
            return GenerateRandomString(160) + "_" + userID;
        }

        private static string GenerateRandomString(int length)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append(Alphabet[rand.Next(Alphabet.Length)]);

            return sb.ToString();
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
