﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using Common;
using Common.Interfaces;
using WebCommon.Properties;

namespace WebCommon.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly IOptions<AppSettings> settings;
        private readonly ILogManager logManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logManager"></param>
        public ExceptionHandlerAttribute(IOptions<AppSettings> settings, ILogManager logManager)
        {
            this.settings = settings;
            this.logManager = logManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            logManager.AddLog(context.Exception);

            HttpStatusCode status;
            string code, message;

            if (context.Exception is NotImplementedException)
            {
                status = HttpStatusCode.NotImplemented;
                code = "MethodNotImplemented";
                message = Resources.errMethodNotImplemented;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                code = "SystemError";
                message = settings.Value.IsProduction ? Resources.errGeneral : logManager.GetExceptionDetails(context.Exception);
            }

            context.HttpContext.Response.StatusCode = (int)status;
            context.Result = new JsonResult(new HttpErrorMessage(code, message));

            base.OnException(context);
        }
    }
}