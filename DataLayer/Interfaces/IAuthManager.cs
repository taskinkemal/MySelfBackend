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
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool VerifyAccessToken(string accessToken, out int userId);

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