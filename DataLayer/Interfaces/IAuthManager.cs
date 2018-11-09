using Common.Interfaces;
using Common.Models;
using Common.Models.Entities;


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
        /// <param name="accessToken"></param>
        /// <returns></returns>
        UserToken VerifyAccessToken(string accessToken);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(int userId, string deviceId);
    }
}