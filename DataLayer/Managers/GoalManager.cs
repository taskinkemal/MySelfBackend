using System;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Common.Extensions;
using System.Threading.Tasks;

namespace DataLayer.Managers
{
    public class GoalManager : ManagerBase, IGoalManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public GoalManager(MyselfContext context) : base(context) { }

        public async Task<int> AddOrUpdateGoal(int userId, Goal goal)
        {
            var existingGoal = await Context.Goals.FindAsync(goal.Id).ConfigureAwait(false);

            if (existingGoal != null)
            {
                var existingTask = await Context.Tasks.FindAsync(existingGoal.TaskId).ConfigureAwait(false);
                if (existingTask.UserId != userId)
                {
                    return -1;
                }

                if (existingGoal.AchievementStatus != 0 || existingGoal.EndDay <= DateTime.Now.GetDay())
                {
                    return -1;
                }

                //if (existingGoal.ModificationDate > goal.ModificationDate)
                //{
                //    return -1;
                //}

                existingGoal.StartDay = goal.StartDay;
                existingGoal.EndDay = goal.EndDay;
                existingGoal.Target = goal.Target;
                existingGoal.MinMax = goal.MinMax;
                existingGoal.ModificationDate = DateTime.Now;

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return existingGoal.Id;
            }
            else
            {
                if (goal.EndDay <= DateTime.Now.GetDay())
                {
                    return -1;
                }

                goal.AchievementStatus = 0;
                var result = await Context.Goals.AddAsync(goal).ConfigureAwait(false);

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return result.Entity.Id;
            }
        }

        public async Task<bool> DeleteGoal(int userId, int id)
        {
            var goal = await GetGoal(userId, id).ConfigureAwait(false);

            if (goal != null)
            {
                Context.Goals.Remove(goal);
                await Context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }

        public async Task<List<Goal>> GetGoals(int userId)
        {
            var result = from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         where task.UserId == userId && task.Status == 1
                         select goal;

            var list = await result.ToListAsync();

            var values = from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         join entry in Context.Entries on goal.TaskId equals entry.TaskId
                         where task.UserId == userId && task.Status == 1
                            && entry.Day >= goal.StartDay && entry.Day <= goal.EndDay
                         group entry by new { goal.Id } into g
                         select new { GoalId = g.Key.Id, Total = g.Sum(e => e.Value) };

            var dictValues = values.ToDictionary(arg => arg.GoalId, arg => arg.Total);

            foreach (var goal in list)
            {
                if (dictValues.ContainsKey(goal.Id))
                {
                    goal.CurrentValue = dictValues[goal.Id];
                }
            }

            return list;
        }

        /// <summary>
        /// Checks the goal achievements.
        /// </summary>
        /// <returns>The goal achievements.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="taskId">Task identifier.</param>
        public async Task<List<int>> CheckGoalAchievements(int userId, int taskId)
        {
            var result = from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         where task.UserId == userId && goal.TaskId == taskId && task.Status == 1
                         && goal.AchievementStatus == 0
                         select goal;

            var list = await result.ToListAsync();

            if (list.Count == 0)
            {
                return new List<int>();
            }

            var minDate = result.Select(x => x.StartDay).Min();
            var maxDate = result.Select(x => x.StartDay).Max();

            var entriesQueryable = from entry in Context.Entries
                         where entry.TaskId == taskId && entry.Day >= minDate && entry.Day <= maxDate
                         select entry;

            var entries = await entriesQueryable.ToListAsync();

            var achievedGoals = new List<int>();

            foreach (var goal in list)
            {
                if (goal.EndDay <= DateTime.Now.GetDay())
                {
                    goal.AchievementStatus = 2;
                    Context.Goals.Update(goal);
                }
                else
                {
                    var sum = entries
                        .Where(e => e.Day >= goal.StartDay && e.Day <= goal.EndDay)
                        .Select(e => e.Value)
                        .Sum();

                    if (goal.MinMax == 1 && goal.Target == sum ||
                        goal.MinMax == 2 && goal.Target <= sum ||
                        goal.MinMax == 3 && goal.Target >= sum)
                    {
                        achievedGoals.Add(goal.Id);

                        goal.AchievementStatus = 1;
                        Context.Goals.Update(goal);
                    }
                    else if (goal.MinMax == 1 && goal.Target < sum ||
                        goal.MinMax == 3 && goal.Target < sum)
                    {
                        goal.AchievementStatus = 2;
                        Context.Goals.Update(goal);
                    }
                }
            }

            return achievedGoals;
        }

        private async Task<Goal> GetGoal(int userId, int id)
        {
            var result = from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         where task.UserId == userId && goal.Id == id
                         select goal;

            var list = await result.ToListAsync();

            return list.Count == 1 ? list[0] : null;
        }
    }
}
