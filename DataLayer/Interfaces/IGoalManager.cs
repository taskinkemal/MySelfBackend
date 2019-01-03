using System;
using System.Collections.Generic;
using Common.Interfaces;
using Common.Models.Entities;

namespace DataLayer.Interfaces
{
    public interface IGoalManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<List<Goal>> GetGoals(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<int> AddOrUpdateGoal(int userId, Goal goal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> DeleteGoal(int userId, int id);

        /// <summary>
        /// Checks the goal achievements.
        /// </summary>
        /// <returns>The goal achievements.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="taskId">Task identifier.</param>
        System.Threading.Tasks.Task<List<int>> CheckGoalAchievements(int userId, int taskId);
    }
}
