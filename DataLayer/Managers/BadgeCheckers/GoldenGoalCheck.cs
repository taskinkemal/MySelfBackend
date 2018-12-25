using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;

namespace DataLayer.Managers.BadgeCheckers
{
    /// <summary>
    /// Golden goal check.
    /// </summary>
    public class GoldenGoalCheck : BadgeCheckBase, IBadgeCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataLayer.Managers.BadgeCheckers.GoldenGoalCheck"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public GoldenGoalCheck(MyselfContext context) : base(context)
        {
        }

        protected override int BadgeId => 3;

        /// <summary>
        /// Gets the new badges.
        /// </summary>
        /// <returns>The new badges.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="existingBadges">All existing badges.</param>
        public async Task<(List<UserBadge> badges, int score)> GetNewBadges(int userId, List<UserBadge> existingBadges)
        {
            var currentLevel = GetCurrentLevel(existingBadges);
            var result = new List<UserBadge>();
            var score = 0;

            var allAchievements = (from achievement in Context.AchievedGoals
                                   join task in Context.Tasks on achievement.TaskId equals task.Id
                                   where task.GoalTimeFrame == 3 && task.UserId == userId && task.Status == 1
                                   select achievement).ToAsyncEnumerable();

            var allAchievementsList = await allAchievements.ToList().ConfigureAwait(false);

            if (currentLevel == 0)
            {
                if (allAchievementsList.Count > 0)
                {
                    AddBadge(result, userId, 1);
                    score += 20;
                }
            }

            if (currentLevel < 2)
            {
                var count = GetConsecutiveMonthAchievementsCount(allAchievementsList);

                if (count >= 1)
                {
                    AddBadge(result, userId, 2);
                    score += 50;
                }
            }

            if (currentLevel < 3)
            {
                var count = GetConsecutiveMonthAchievementsCount(allAchievementsList);

                if (count >= 3)
                {
                    AddBadge(result, userId, 3);
                    score += 100;
                }
            }

            return (badges: result, score: score);
        }

        private int GetConsecutiveMonthAchievementsCount(List<AchievedGoal> allAchievements)
        {
            var orderedList = allAchievements.OrderBy(a => a.TaskId).ThenBy(a => a.Day);

            var count = 0;
            var months = 0;

            for (var i = 0; i < allAchievements.Count; i++)
            {
                if (i == 0 || allAchievements[i - 1].TaskId != allAchievements[i].TaskId)
                {
                    months = 0;
                }
                else
                {
                    if (allAchievements[i].Day.GetDate() == allAchievements[i - 1].Day.GetDate().AddMonths(1))
                    {
                        months++;
                    }
                }

                if (i == allAchievements.Count - 1 || allAchievements[i + 1].TaskId != allAchievements[i].TaskId)
                {
                    if (months >= 3)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
