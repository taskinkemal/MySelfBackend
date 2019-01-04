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
        public async System.Threading.Tasks.Task<List<UserBadge>> GetUserBadges(int userId)
        {
            var result = await (from userBadge in Context.UserBadges
                         where userBadge.UserId == userId
                         select userBadge).ToListAsync();

            return result;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userId">User identifier.</param>
        public async System.Threading.Tasks.Task<User> GetUser(int userId)
        {
            var result = await (from user in Context.Users
                         where user.Id == userId
                         select user).FirstOrDefaultAsync();

            if (result != null)
            {
                result.Badges = await GetUserBadges(userId);
            }

            return result;
        }
    }
}
