using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

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

            var allAchievedGoals = await (from goal in Context.Goals
                            join task in Context.Tasks on goal.TaskId equals task.Id
                            where task.UserId == userId && task.Status == 1 && goal.AchievementStatus == 1
                            select goal).ToListAsync();

            if (currentLevel == 0)
            {
                if (allAchievedGoals.Count > 0)
                {
                    AddBadge(result, userId, 1);
                    score += 20;
                }
            }

            if (currentLevel < 2)
            {
                if (allAchievedGoals.Count >= 5)
                {
                    AddBadge(result, userId, 2);
                    score += 50;
                }
            }

            if (currentLevel < 3)
            {
                if (allAchievedGoals.Count >= 25)
                {
                    AddBadge(result, userId, 3);
                    score += 100;
                }
            }

            return (badges: result, score: score);
        }
    }
}
