using System;
using Common.Models.Entities;
using DataLayer.Context;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Common.Models;
using Common.Extensions;

namespace DataLayer.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class EntryManager : ManagerBase, IEntryManager
    {
        private readonly IGoalManager goalManager;
        private readonly IEnumerable<IBadgeCheck> badgeCheckers;

        public EntryManager(MyselfContext context, IGoalManager goalManager, IEnumerable<IBadgeCheck> badgeCheckers) : base(context)
        {
            this.goalManager = goalManager;
            this.badgeCheckers = badgeCheckers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<List<Entry>> GetEntries(int userId, int start, int end)
        {
            var result = await (from entry in Context.Entries
                         join task in Context.Tasks on entry.TaskId equals task.Id
                         where entry.Day >= start && entry.Day <= end && task.UserId == userId && task.Status == 1
                         select entry).ToListAsync().ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<UploadEntryResponse> AddOrUpdateEntry(int userId, Entry entry)
        {
            var existingEntry = await Context.Entries.FindAsync(entry.Day, entry.TaskId).ConfigureAwait(false);
            var task = await Context.Tasks.FindAsync(entry.TaskId).ConfigureAwait(false);
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            var existingBadges = await Context.UserBadges.Where(u => u.UserId == userId).ToListAsync().ConfigureAwait(false);

            if (task != null && task.UserId == userId && entry.Value >= 0)
            {
                if (task.DataType == 0 && entry.Value > 1)
                {
                    return null;
                }

                var result = new UploadEntryResponse();

                if (existingEntry != null)
                {
                    //if (existingEntry.ModificationDate > entry.ModificationDate)
                    //{
                    //    return false;
                    //}

                    existingEntry.Value = entry.Value;
                    existingEntry.ModificationDate = DateTime.Now;
                }
                else
                {
                    await Context.Entries.AddAsync(new Entry
                    {
                        Day = entry.Day,
                        TaskId = entry.TaskId,
                        Value = entry.Value
                    }).ConfigureAwait(false);

                    if (user != null)
                    {
                        user.Score += 1;

                        Context.Users.Update(user);
                    }
                }

                if (user != null)
                {
                    result.Score = user.Score;
                }

                await goalManager.CheckGoalAchievements(userId, task.Id).ConfigureAwait(false);

                foreach (var checker in badgeCheckers)
                {
                    var list = await checker.GetNewBadges(userId, existingBadges).ConfigureAwait(false);

                    async void action(UserBadge badge)
                    {
                        await AddOrUpdateBadgeAsync(userId, badge.BadgeId, badge.Level).ConfigureAwait(false);
                    }
                    list.badges.ForEach(action);
                    user.Score += list.score;
                    result.Score += list.score;

                    result.NewBadges.AddRange(list.badges);
                }

                await Context.SaveChangesAsync().ConfigureAwait(false);

                return result;
            }

            return null;
        }

        private async System.Threading.Tasks.Task<bool> AddOrUpdateBadgeAsync(int userId, int badgeId, int level)
        {
            var existingBadge = await Context.UserBadges.FindAsync(userId, badgeId).ConfigureAwait(false);

            if (existingBadge == null)
            {
                await Context.UserBadges.AddAsync(new UserBadge
                {
                    UserId = userId,
                    BadgeId = badgeId,
                    Level = level,
                    ModificationDate = DateTime.Now
                }).ConfigureAwait(false);

                return true;
            }
            else if (existingBadge.Level < level)
            {
                existingBadge.Level = level;
                existingBadge.ModificationDate = DateTime.Now;

                return true;
            }

            return false;
        }
    }
}
