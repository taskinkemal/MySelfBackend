using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models.Entities;
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
            return Context.Tasks.Where(t => t.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> AddOrUpdateTask(int userId, Task task)
        {
            var existingTask = await Context.Tasks.FindAsync(task.Id).ConfigureAwait(false);

            if (existingTask != null)
            {
                if (existingTask.UserId != userId)
                {
                    return -1;
                }

                //if (existingTask.ModificationDate > task.ModificationDate)
                //{
                //    return -1;
                //}

                existingTask.Label = task.Label;
                existingTask.DataType = task.DataType;
                existingTask.Unit = task.Unit;
                existingTask.HasGoal = task.HasGoal;
                existingTask.Goal = task.Goal;
                existingTask.GoalMinMax = task.GoalMinMax;
                existingTask.GoalTimeFrame = task.GoalTimeFrame;
                existingTask.AutomationType = task.AutomationType;
                existingTask.AutomationVar = task.AutomationVar;
                existingTask.ModificationDate = DateTime.Now;

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return existingTask.Id;
            }
            else
            {
                task.UserId = userId;
                task.Status = 1;
                var result = await Context.Tasks.AddAsync(task).ConfigureAwait(false);

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return result.Entity.Id;
            }
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
