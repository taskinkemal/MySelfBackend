using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Common;
using WebCommon.Properties;
using WebCommon.BaseControllers;
using Common.Models;
using DataLayer.Interfaces;


namespace WebCommon.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionFilterAttribute : ActionFilterAttribute
    {
        private readonly IAuthManager authManager;
        private readonly bool authenticationRequired;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authManager"></param>
        /// <param name="authenticationRequired"></param>
        public ExecutionFilterAttribute(IAuthManager authManager, bool authenticationRequired)
        {
            this.authManager = authManager;
            this.authenticationRequired = authenticationRequired;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RetrieveParameters(context, out var accessToken);
            SetCulture();

            var isValidated = ValidateRequest(context, accessToken, out var errPhrase, out var errMessage);

            if (isValidated || !authenticationRequired)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new JsonResult(new HttpErrorMessage(errPhrase, errMessage))
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    var obj = (context.Result as JsonResult)?.Value;

        //    if (obj != null)
        //    {
        //        var rootFieldName = ClassNameHelper.GetRootFieldName(obj.GetType());

        //        if (!string.IsNullOrWhiteSpace(rootFieldName))
        //        {
        //            var data = new JObject
        //            {
        //                { rootFieldName, JObject.FromObject(obj) }
        //            };

        //            (context.Result as JsonResult).Value = data;
        //        }
        //    }
        //    else
        //    {
        //        obj = (context.Result as ObjectResult)?.Value;

        //        if (obj != null)
        //        {
        //            var objType = obj.GetType();

        //            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
        //            {
        //                // FNM: better do this only in SQL part, and remove that last chance check
        //                (context.Result as ObjectResult).Value = (obj as IEnumerable<dynamic>)?.Take(AppSettings.MaxRows);
        //            }
        //        }
        //        else
        //        {
        //            if (context.Result is JsonResult)
        //            {
        //                (context.Result as JsonResult).StatusCode = (int)HttpStatusCode.NotFound;
        //                (context.Result as JsonResult).Value = new HttpErrorMessage("ItemNotFound", Resources.errItemNotFound);
        //            }
        //            else if (context.Result is ObjectResult)
        //            {
        //                (context.Result as ObjectResult).StatusCode = (int)HttpStatusCode.NotFound;
        //                (context.Result as ObjectResult).Value = new HttpErrorMessage("ItemNotFound", Resources.errItemNotFound);
        //            }
        //        }
        //    }

        //    base.OnResultExecuting(context);
        //}

        private bool ValidateRequest(ActionExecutingContext context, string accessToken, out string errPhrase, out string errMessage)
        {
            errPhrase = errMessage = "";

            if (accessToken != null)
            {
                if (context.Controller is BaseController apiController)
                {
                    var authToken = new AuthToken(accessToken, DateTime.Now.AddYears(1));

                    if (authManager.IsTokenValid(authToken))
                    {
                        apiController.Token = authToken;
                    }
                    else
                    {
                        errPhrase = "InvalidToken";
                        errMessage = Resources.errInvalidToken;
                        return false;
                    }
                }
                else
                {
                    errPhrase = "InvalidController";
                    errMessage = Resources.errInvalidController;
                    return false;
                }
            }
            else
            {
                errPhrase = "InvalidToken";
                errMessage = Resources.errInvalidToken;
                return false;
            }

            return true;
        }

        private static void RetrieveParameters(ActionContext context, out string accessToken)
        {
            accessToken = GetHeader(context.HttpContext.Request.Headers, "Authorization", "");
            if (!string.IsNullOrWhiteSpace(accessToken) && accessToken.StartsWith("Bearer ", StringComparison.InvariantCulture))
            {
                accessToken = accessToken.Substring("Bearer ".Length);
            }
            else
            {
                accessToken = null;
            }
        }

        private static string GetHeader(IHeaderDictionary headers, string key, string defaultValue)
        {
            return headers.ContainsKey(key) ? Convertor.ToString(headers[key].First(), defaultValue) : defaultValue;
        }

        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }
    }
}
