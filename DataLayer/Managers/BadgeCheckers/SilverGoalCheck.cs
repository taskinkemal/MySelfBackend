using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;

namespace DataLayer.Managers.BadgeCheckers
{
    /// <summary>
    /// Silver goal check.
    /// </summary>
    public class SilverGoalCheck : BadgeCheckBase, IBadgeCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataLayer.Managers.BadgeCheckers.SilverGoalCheck"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SilverGoalCheck(MyselfContext context) : base(context)
        {
        }

        protected override int BadgeId => 2;

        /// <summary>
        /// Gets the new badges async.
        /// </summary>
        /// <returns>The new badges async.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="existingBadges">Existing badges.</param>
        public async Task<(List<UserBadge> badges, int score)> GetNewBadges(int userId, List<UserBadge> existingBadges)
        {
            var currentLevel = GetCurrentLevel(existingBadges);
            var result = new List<UserBadge>();
            var score = 0;

            var allAchievements = (from achievement in Context.AchievedGoals
                                        join task in Context.Tasks on achievement.TaskId equals task.Id
                                        where task.GoalTimeFrame == 2 && task.UserId == userId && task.Status == 1
                                        select achievement).ToAsyncEnumerable();

            var allAchievementsList = await allAchievements.ToList().ConfigureAwait(false);

            if (currentLevel == 0)
            {
                if (allAchievementsList.Count > 0)
                {
                    AddBadge(result, userId, 1);
                    score += 10;
                }
            }

            if (currentLevel < 2)
            {
                var count = GetConsecutiveWeekAchievementsCount(allAchievementsList);

                if (count >= 1)
                {
                    AddBadge(result, userId, 2);
                    score += 30;
                }
            }

            if (currentLevel < 3)
            {
                var count = GetConsecutiveWeekAchievementsCount(allAchievementsList);

                if (count >= 3)
                {
                    AddBadge(result, userId, 3);
                    score += 60;
                }
            }

            return (badges: result, score: score);
        }

        private int GetConsecutiveWeekAchievementsCount(List<AchievedGoal> allAchievements)
        {
            var orderedList = allAchievements.OrderBy(a => a.TaskId).ThenBy(a => a.Day);

            var count = 0;
            var weeks = 0;

            for (var i = 0; i < allAchievements.Count; i++)
            {
                if (i == 0 || allAchievements[i - 1].TaskId != allAchievements[i].TaskId)
                {
                    weeks = 0;
                }
                else
                {
                    if (allAchievements[i].Day == allAchievements[i - 1].Day + 7)
                    {
                        weeks++;
                    }
                }

                if (i == allAchievements.Count - 1 || allAchievements[i + 1].TaskId != allAchievements[i].TaskId)
                {
                    if (weeks >= 5)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
