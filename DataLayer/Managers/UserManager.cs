using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Managers
{
    /// <summary>
    /// User manager.
    /// </summary>
    public class UserManager : ManagerBase, IUserManager
    {
        public UserManager(MyselfContext context) : base(context) { }

        /// <summary>
        /// Gets the user badges.
        /// </summary>
        /// <returns>The user badges.</returns>
        /// <param name="userId">User identifier.</param>
        public System.Threading.Tasks.Task<List<UserBadge>> GetUserBadges(int userId)
        {
            var result = from userBadge in Context.UserBadges
                         where userBadge.UserId == userId
                         select userBadge;

            return result.ToListAsync();
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userId">User identifier.</param>
        public async System.Threading.Tasks.Task<User> GetUser(int userId)
        {
            var result = (from user in Context.Users
                         where user.Id == userId
                         select user).FirstOrDefault();

            if (result != null)
            {
                result.Badges = await GetUserBadges(userId).ConfigureAwait(false);
            }

            return result;
        }
    }
}
