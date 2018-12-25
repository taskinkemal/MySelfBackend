using System.Net;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models;
using Common.Models.Entities;
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
        public async Task<GenericCollection<Entry>> Get(int start, int end)
        {
            var list = await entryManager.GetEntries(Token.UserID, start, end).ConfigureAwait(false);

            return list.ToCollection();
        }

        /// <summary>
        /// Inserts or updates the entry.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Http response.</returns>
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]Entry data)
        {
            var result = await entryManager.AddOrUpdateEntry(Token.UserID, data).ConfigureAwait(false);

            if (result != null)
            {
                return CreateResponse(result);
            }
            else
            {
                return CreateErrorResponse(HttpStatusCode.InternalServerError, "UpdateFailed", "Entry cannot be updated.");
            }
        }
    }
}
