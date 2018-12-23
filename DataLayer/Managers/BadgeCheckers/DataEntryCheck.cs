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
    /// Data entry check.
    /// </summary>
    public class DataEntryCheck : BadgeCheckBase, IBadgeCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataLayer.Managers.BadgeCheckers.DataEntryCheck"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public DataEntryCheck(MyselfContext context) : base(context)
        {
        }

        protected override int BadgeId => 1;

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

            if (currentLevel == 0)
            {
                var success = await CheckLevel1(userId).ConfigureAwait(false);

                if (success)
                {
                    AddBadge(result, userId, 1);
                    score += 10;
                }
            }

            if (currentLevel < 2)
            {
                var success = await CheckLevel2(userId).ConfigureAwait(false);

                if (success)
                {
                    AddBadge(result, userId, 2);
                    score += 20;
                }
            }

            if (currentLevel < 3)
            {
                var success = await CheckLevel3(userId).ConfigureAwait(false);

                if (success)
                {
                    AddBadge(result, userId, 3);
                    score += 50;
                }
            }

            return (badges: result, score: score);
        }

        private async Task<bool> CheckLevel1(int userId)
        {
            var result = new List<UserBadge>();

            var entries = from entry in Context.Entries
                          join task in Context.Tasks on entry.TaskId equals task.Id
                          where task.UserId == userId && task.Status == 1
                          group entry.TaskId by entry.Day into g
                          select new
                          {
                              g.Key,
                              Entries = g.Count()
                          };

            var e = await entries.ToListAsync().ConfigureAwait(false);
            return e.Count == 5;
        }

        private async Task<bool> CheckLevel2(int userId)
        {
            var result = await CheckConsecutiveEntries(userId, 5);

            return result == 5;
        }

        private async Task<bool> CheckLevel3(int userId)
        {
            var result = await CheckConsecutiveEntries(userId, 30);

            return result == 30;
        }

        private async Task<int> CheckConsecutiveEntries(int userId, int days)
        {
            var result = new List<UserBadge>();

            var today = DateTime.Now.GetDay();
            var entries = from entry in Context.Entries
                          join task in Context.Tasks on entry.TaskId equals task.Id
                          where entry.Day >= (today - days) && entry.Day <= today && task.UserId == userId && task.Status == 1
                          group entry.TaskId by entry.Day into g
                          select new
                          {
                              g.Key,
                              Entries = g.Count()
                          };

            var e = await entries.ToListAsync().ConfigureAwait(false);
            return e.Count;
        }
    }
}
