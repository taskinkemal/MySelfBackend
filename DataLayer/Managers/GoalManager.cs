using System;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Common.Extensions;
using System.Threading.Tasks;
using Common.Models;

namespace DataLayer.Managers
{
    public class GoalManager : ManagerBase, IGoalManager
    {
        private readonly IEnumerable<IBadgeCheck> badgeCheckers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="badgeCheckers"></param>
        public GoalManager(MyselfContext context, IEnumerable<IBadgeCheck> badgeCheckers) : base(context)
        {
            this.badgeCheckers = badgeCheckers;
        }

        public async Task<UploadGoalResponse> AddOrUpdateGoal(int userId, Goal goal)
        {
            var existingGoal = await Context.Goals.FindAsync(goal.Id);

            if (existingGoal != null)
            {
                var existingTask = await Context.Tasks.FindAsync(existingGoal.TaskId);
                if (existingTask.UserId != userId)
                {
                    return null;
                }

                if (existingGoal.AchievementStatus != 0 || existingGoal.EndDay <= DateTime.Now.GetDay())
                {
                    return null;
                }

                //if (existingGoal.ModificationDate > goal.ModificationDate)
                //{
                //    return null;
                //}

                existingGoal.StartDay = goal.StartDay;
                existingGoal.EndDay = goal.EndDay;
                existingGoal.Target = goal.Target;
                existingGoal.MinMax = goal.MinMax;
                existingGoal.ModificationDate = DateTime.Now;

                Context.Goals.Update(existingGoal);

                await Context.SaveChangesAsync();

                var response = await GetUploadResponse(userId, existingGoal.Id);

                await UpdateUserScore(userId, response.Score);

                return response;
            }
            else
            {
                if (goal.EndDay <= DateTime.Now.GetDay())
                {
                    return null;
                }

                goal.AchievementStatus = 0;
                var result = Context.Goals.Add(goal);

                await Context.SaveChangesAsync();

                var response = await GetUploadResponse(userId, result.Entity.Id);

                await UpdateUserScore(userId, response.Score);

                return response;
            }
        }

        public async Task<bool> DeleteGoal(int userId, int id)
        {
            var goal = await GetGoal(userId, id);

            if (goal != null)
            {
                Context.Goals.Remove(goal);
                await Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<Goal>> GetGoals(int userId)
        {
            var list = await (from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         where task.UserId == userId && task.Status == 1
                         select goal).ToListAsync();

            var values = await (from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         join entry in Context.Entries on goal.TaskId equals entry.TaskId
                         where task.UserId == userId && task.Status == 1
                            && entry.Day >= goal.StartDay && entry.Day <= goal.EndDay
                         group entry by new { goal.Id } into g
                         select new { GoalId = g.Key.Id, Total = g.Sum(e => e.Value) }).ToListAsync();

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
            var list = await (from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         where task.UserId == userId && goal.TaskId == taskId && task.Status == 1
                         && goal.AchievementStatus == 0
                         select goal).ToListAsync();

            if (list.Count == 0)
            {
                return new List<int>();
            }

            var minDate = list.Select(x => x.StartDay).Min();
            var maxDate = list.Select(x => x.EndDay).Max();

            var entries = await (from entry in Context.Entries
                         where entry.TaskId == taskId && entry.Day >= minDate && entry.Day <= maxDate
                         select entry).ToListAsync();

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
                        goal.MinMax == 3 && goal.Target >= sum && goal.EndDay < DateTime.Now.GetDay())
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

        public async Task<(int Score, List<UserBadge> NewBadges)> CheckNewBadges(int userId)
        {
            var score = 0;
            var newBadges = new List<UserBadge>();

            var existingBadges = await Context.UserBadges.Where(u => u.UserId == userId).ToListAsync();

            foreach (var checker in badgeCheckers)
            {
                var list = await checker.GetNewBadges(userId, existingBadges);

                foreach (var badge in list.badges)
                {
                    await AddOrUpdateBadgeAsync(userId, badge.BadgeId, badge.Level);
                }

                score += list.score;

                newBadges.AddRange(list.badges);
            }

            return (Score: score, NewBadges: newBadges);
        }

        private async Task<UploadGoalResponse> GetUploadResponse(int userId, int goalId)
        {
            var checkBadgesResult = await CheckNewBadges(userId);

            return new UploadGoalResponse
            {
                GoalId = goalId,
                Score = checkBadgesResult.Score,
                NewBadges = checkBadgesResult.NewBadges
            };
        }

        private async Task<bool> UpdateUserScore(int userId, int score)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.Score += score;
                Context.Users.Update(user);
                await Context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        private async Task<bool> AddOrUpdateBadgeAsync(int userId, int badgeId, int level)
        {
            var existingBadge = await Context.UserBadges.FindAsync(userId, badgeId);

            if (existingBadge == null)
            {
                Context.UserBadges.Add(new UserBadge
                {
                    UserId = userId,
                    BadgeId = badgeId,
                    Level = level,
                    ModificationDate = DateTime.Now
                });

                return true;
            }
            else if (existingBadge.Level < level)
            {
                existingBadge.Level = level;
                existingBadge.ModificationDate = DateTime.Now;

                Context.UserBadges.Update(existingBadge);

                return true;
            }

            return false;
        }

        private async Task<Goal> GetGoal(int userId, int id)
        {
            var list = await (from goal in Context.Goals
                         join task in Context.Tasks on goal.TaskId equals task.Id
                         where task.UserId == userId && goal.Id == id
                         select goal).ToListAsync();

            return list.Count == 1 ? list[0] : null;
        }
    }
}
