using System.Net;
using System.Threading.Tasks;
using Common.Models;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;
using WebCommon.BaseControllers;


namespace WebApplication.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class TokenController : NoAuthController
    {
        private readonly IAuthManager authManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authManager"></param>
        public TokenController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        /// <summary>
        /// Verifies the token and returns a new one.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Access token.</returns>
        [HttpPost("/api/Token/Refresh")]
        public async Task<JsonResult> PostRefreshToken([FromBody]TokenRefreshRequest data)
        {
            if (!string.IsNullOrWhiteSpace(data?.AccessToken))
            {
                var userToken = await authManager.VerifyAccessToken(data.AccessToken);
                if (userToken != null)
                {
                    var result = await authManager.GenerateTokenAsync(userToken.UserId, data.DeviceID);
                    return CreateResponse(result);
                }
            }

            return CreateErrorResponse(HttpStatusCode.Unauthorized, "InvalidCredentials", /*Resources.errLogin_General*/ "A general error has occured.");
        }

        /// <summary>
        /// Generates and returns an access token for user.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Access token.</returns>
        [HttpPost("/api/Token/Facebook")]
        public async Task<JsonResult> PostFacebook([FromBody]TokenRequest data)
        {
            var userID = -1;
            var isValidated = false;
            JsonResult response;

            if (!string.IsNullOrWhiteSpace(data?.FacebookToken) && !string.IsNullOrWhiteSpace(data.Email))
            {
                userID = await authManager.VerifyFacebookUserAsync(data.ToUser(), data.FacebookToken);

                isValidated = userID >= 0;
            }

            if (isValidated)
            {
                var result = await authManager.GenerateTokenAsync(userID, data.DeviceID);
                response = CreateResponse(result);
            }
            else
            {
                response = CreateErrorResponse(HttpStatusCode.Unauthorized, "InvalidCredentials", /*Resources.errLogin_General*/ "A general error has occured.");
            }

            return response;
        }

        /// <summary>
        /// Generates and returns an access token for user.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Access token.</returns>
        [HttpPost("/api/Token/Google")]
        public async Task<JsonResult> PostGoogle([FromBody]TokenRequest data)
        {
            var userID = -1;
            var isValidated = false;
            JsonResult response;

            if (!string.IsNullOrWhiteSpace(data?.GoogleToken) && !string.IsNullOrWhiteSpace(data.Email))
            {
                userID = await authManager.VerifyGoogleUserAsync(data.ToUser(), data.GoogleToken);

                isValidated = userID >= 0;
            }

            if (isValidated)
            {
                var result = await authManager.GenerateTokenAsync(userID, data.DeviceID);
                response = CreateResponse(result);
            }
            else
            {
                response = CreateErrorResponse(HttpStatusCode.Unauthorized, "InvalidCredentials", /*Resources.errLogin_General*/ "A general error has occured.");
            }

            return response;
        }

        /// <summary>
        /// Deletes and invalidates token.
        /// </summary>
        [TypeFilter(typeof(ExecutionFilterAttribute), Arguments = new object[] { true })]
        [HttpDelete]
        public JsonResult Delete()
        {
            return CreateErrorResponse(HttpStatusCode.BadRequest, "NoAction", "");
        }
    }
}
