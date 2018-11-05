using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common.Models;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;


namespace WebApplication.Controllers
{
    public class EntriesController : AuthController
    {
        private readonly IEntryManager entryManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryManager"></param>
        public EntriesController(IEntryManager entryManager)
        {
            this.entryManager = entryManager;
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>List of entries.</returns>
        [HttpGet]
        public Task<List<Entry>> Get(int start, int end)
        {
            return entryManager.GetEntries(Token.UserID, start, end);
        }

        /// <summary>
        /// Inserts or updates the entry.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Http response.</returns>
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]Entry data)
        {
            var result = await entryManager.AddOrUpdateEntry(Token.UserID, data);

            var response = result ?
                CreateResponse(true) :
                CreateErrorResponse(HttpStatusCode.InternalServerError, "UpdateFailed", "Entry cannot be updated.");

            return response;
        }
    }
}
