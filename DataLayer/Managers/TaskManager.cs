using System.Collections.Generic;
using System.Linq;
using Common.Models;
using DataLayer.Context;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DataLayer.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskManager : ManagerBase, ITaskManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public TaskManager(MyselfContext context) : base(context) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<List<Task>> GetTasks(int userId)
        {
            return Context.Tasks.Where(t => t.UserId == userId && t.Status == 1).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> AddOrUpdateTask(int userId, Task task)
        {
            var existingTask = await Context.Tasks.FindAsync(task.Id).ConfigureAwait(false);

            if (existingTask != null)
            {
                if (existingTask.UserId != userId)
                {
                    return false;
                }

                existingTask.Label = task.Label;
                existingTask.DataType = task.DataType;
                existingTask.Unit = task.Unit;
                existingTask.HasGoal = task.HasGoal;
                existingTask.Goal = task.Goal;
                existingTask.GoalMinMax = task.GoalMinMax;
                existingTask.GoalTimeFrame = task.GoalTimeFrame;

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return true;
            }
            else
            {
                task.UserId = userId;
                await Context.Tasks.AddAsync(task).ConfigureAwait(false);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> DeleteTask(int userId, int id)
        {
            var task = await Context.Tasks.FindAsync(id).ConfigureAwait(false);

            if (task != null && task.UserId == userId)
            {
                task.Status = 0;
                await Context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }
    }
}
