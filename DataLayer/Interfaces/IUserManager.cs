using System.Collections.Generic;
using Common.Models.Entities;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// User manager.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Gets the user badges.
        /// </summary>
        /// <returns>The user badges.</returns>
        /// <param name="userId">User identifier.</param>
        System.Threading.Tasks.Task<List<UserBadge>> GetUserBadges(int userId);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userId">User identifier.</param>
        System.Threading.Tasks.Task<User> GetUser(int userId);
    }
}
