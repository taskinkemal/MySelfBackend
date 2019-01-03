using System.Collections.Generic;
using Common.Interfaces;
using Common.Models.Entities;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Badge check.
    /// </summary>
    public interface IBadgeCheck : IDependency
    {
        /// <summary>
        /// Gets the new badges.
        /// </summary>
        /// <returns>The new badges.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="existingBadges">All existing badges.</param>
        System.Threading.Tasks.Task<(List<UserBadge> badges, int score)> GetNewBadges(int userId, List<UserBadge> existingBadges);
    }
}
