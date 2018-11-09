using System.Collections.Generic;
using Common.Interfaces;
using Common.Models.Entities;


namespace DataLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITaskManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<List<Task>> GetTasks(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<int> AddOrUpdateTask(int userId, Task task);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> DeleteTask(int userId, int id);
    }
}
