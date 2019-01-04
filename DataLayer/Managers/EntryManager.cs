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

        public EntryManager(MyselfContext context, IGoalManager goalManager) : base(context)
        {
            this.goalManager = goalManager;
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
                         select entry).ToListAsync();

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
            var existingEntry = await Context.Entries.FindAsync(entry.Day, entry.TaskId);
            var task = await Context.Tasks.FindAsync(entry.TaskId);
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userId);

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

                    Context.Entries.Update(existingEntry);
                }
                else
                {
                    Context.Entries.Add(new Entry
                    {
                        Day = entry.Day,
                        TaskId = entry.TaskId,
                        Value = entry.Value
                    });

                    user.Score += 1;
                }

                await goalManager.CheckGoalAchievements(userId, task.Id);

                var checkBadgesResult = await goalManager.CheckNewBadges(userId);

                user.Score += checkBadgesResult.Score;
                result.NewBadges.AddRange(checkBadgesResult.NewBadges);
                result.Score = user.Score;
                Context.Users.Update(user);

                await Context.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}
