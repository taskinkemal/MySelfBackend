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

            if (currentLevel == 0)
            {

            }

            if (currentLevel < 2)
            {

            }

            if (currentLevel < 3)
            {

            }

            return (badges: new List<UserBadge>(), score: 0);
        }
    }
}
