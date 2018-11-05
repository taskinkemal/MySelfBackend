using System;
using Common.Interfaces;
using Common.Models;


namespace DataLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="validUntil"></param>
        /// <returns></returns>
        AuthToken GenerateToken(int userID, DateTime validUntil);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsTokenValid(AuthToken token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<int> VerifyFacebookUserAsync(string email, string accessToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<int> VerifyGoogleUserAsync(string email, string accessToken);
    }
}