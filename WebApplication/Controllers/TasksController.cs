using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebCommon.BaseControllers;


namespace WebApplication.Controllers
{
    public class TasksController : AuthController
    {
        private readonly ITaskManager taskManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskManager"></param>
        public TasksController(ITaskManager taskManager)
        {
            this.taskManager = taskManager;
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <returns>List of tasks.</returns>
        [HttpGet]
        public Task<List<Common.Models.Task>> Get()
        {
            return taskManager.GetTasks(Token.UserID);
        }

        /// <summary>
        /// Inserts or updates the task.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Http response.</returns>
        [HttpPost]
        public async Task<JsonResult> Post([FromBody]Common.Models.Task data)
        {
            var result = await taskManager.AddOrUpdateTask(Token.UserID, data);

            var response = result ?
                CreateResponse(true) :
                CreateErrorResponse(HttpStatusCode.InternalServerError, "UpdateFailed", "Task cannot be updated.");

            return response;
        }

        /// <summary>
        /// Deletes the task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Http response.</returns>
        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await taskManager.DeleteTask(Token.UserID, id);

            var response = result ?
                CreateResponse(true) :
                CreateErrorResponse(HttpStatusCode.InternalServerError, "DeleteFailed", "Task cannot be deleted.");

            return response;
        }
    }
}
