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
        public async System.Threading.Tasks.Task<List<Task>> GetTasks(int userId)
        {
            var list = await Context.Tasks.Where(t => t.UserId == userId).ToListAsync();

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<int> AddOrUpdateTask(int userId, Task task)
        {
            var existingTask = await Context.Tasks.FindAsync(task.Id);

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
                existingTask.AutomationType = task.AutomationType;
                existingTask.AutomationVar = task.AutomationVar;
                existingTask.ModificationDate = DateTime.Now;

                Context.Tasks.Update(existingTask);

                await Context.SaveChangesAsync();

                return existingTask.Id;
            }
            else
            {
                task.UserId = userId;
                task.Status = 1;
                var result = Context.Tasks.Add(task);

                await Context.SaveChangesAsync();

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
            var task = await Context.Tasks.FindAsync(id);

            if (task != null && task.UserId == userId)
            {
                task.Status = 0;
                await Context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
