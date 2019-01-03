using System;
using System.Net;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;

namespace WebApplication.Controllers
{
    public class GoalsController : AuthController
    {
        private readonly IGoalManager goalManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goalManager"></param>
        public GoalsController(IGoalManager goalManager)
        {
            this.goalManager = goalManager;
        }

        /// <summary>
        /// Gets the goals.
        /// </summary>
        /// <returns>List of goals.</returns>
        [HttpGet]
        public async Task<GenericCollection<Common.Models.Entities.Goal>> Get()
        {
            var list = await goalManager.GetGoals(Token.UserID).ConfigureAwait(false);

            return list.ToCollection();
        }

        /// <summary>
        /// Inserts or updates the goal.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Http response.</returns>
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]Common.Models.Entities.Goal data)
        {
            var result = await goalManager.AddOrUpdateGoal(Token.UserID, data);

            var response = result > -1 ?
                CreateResponse(result) :
                CreateErrorResponse(HttpStatusCode.InternalServerError, "UpdateFailed", "Goal cannot be updated.");

            return response;
        }

        /// <summary>
        /// Deletes the goal.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Http response.</returns>
        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await goalManager.DeleteGoal(Token.UserID, id);

            var response = result ?
                CreateResponse(true) :
                CreateErrorResponse(HttpStatusCode.InternalServerError, "DeleteFailed", "Goal cannot be deleted.");

            return response;
        }
    }
}
