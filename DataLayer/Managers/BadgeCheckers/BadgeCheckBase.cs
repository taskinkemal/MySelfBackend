using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models.Entities;
using DataLayer.Context;

namespace DataLayer.Managers.BadgeCheckers
{
    public abstract class BadgeCheckBase : ManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataLayer.Managers.BadgeCheckers.BadgeCheckBase"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        protected BadgeCheckBase(MyselfContext context) : base(context) { }

        /// <summary>
        /// Gets the badge identifier.
        /// </summary>
        /// <value>The badge identifier.</value>
        protected abstract int BadgeId { get; }

        protected int GetCurrentLevel(List<UserBadge> existingBadges)
        {
            return existingBadges.Where(b => b.BadgeId == BadgeId).Max(b => b.Level);
        }

        protected void AddBadge(List<UserBadge> badges, int userId, int level)
        {
            badges.Add(new UserBadge
            {
                UserId = userId,
                BadgeId = BadgeId,
                Level = level
            });
        }
    }
}
