using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

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

            var allGoals = await (from goal in Context.Goals
                                  join task in Context.Tasks on goal.TaskId equals task.Id
                                  where task.UserId == userId && task.Status == 1
                                  select goal).ToListAsync();

            if (currentLevel == 0)
            {
                if (allGoals.Count > 0)
                {
                    AddBadge(result, userId, 1);
                    score += 10;
                }
            }

            if (currentLevel < 2)
            {
                if (allGoals.Count >= 5)
                {
                    AddBadge(result, userId, 2);
                    score += 30;
                }
            }

            if (currentLevel < 3)
            {
                if (allGoals.Count >= 25)
                {
                    AddBadge(result, userId, 3);
                    score += 60;
                }
            }

            return (badges: result, score: score);
        }
    }
}
