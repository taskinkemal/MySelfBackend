using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;
using System.Net;
using Common;
using Common.Models;

namespace WebCommon.BaseControllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [TypeFilter(typeof(ExceptionHandlerAttribute))]
    public class BaseController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthToken Token = null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected static JsonResult CreateResponse<T>(T result)
        {
            return new JsonResult(result)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected static JsonResult CreateErrorResponse(HttpStatusCode statusCode, string code, string message)
        {
            return new JsonResult(new HttpErrorMessage(code, message))
            {
                StatusCode = (int)statusCode
            };
        }
    }
}
