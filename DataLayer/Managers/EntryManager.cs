using Common.Models;
using DataLayer.Context;
using DataLayer.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace DataLayer.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class EntryManager : ManagerBase, IEntryManager
    {
        public EntryManager(MyselfContext context) : base(context) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<List<Entry>> GetEntries(int userId, int start, int end)
        {
            var result = from entry in Context.Entries
                         join task in Context.Tasks on entry.TaskId equals task.Id
                         where entry.Day >= start && entry.Day <= end && task.UserId == userId && task.Status == 1
                         select entry;

            return result.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> AddOrUpdateEntry(int userId, Entry entry)
        {
            var existingEntry = await Context.Entries.FindAsync(entry.Day, entry.TaskId).ConfigureAwait(false);
            var task = await Context.Tasks.FindAsync(entry.TaskId).ConfigureAwait(false);

            if (task != null && task.UserId == userId && entry.Value >= 0)
            {
                if (task.DataType == 0 && entry.Value > 1)
                {
                    return false;
                }

                if (existingEntry != null)
                {
                    existingEntry.Value = entry.Value;

                    await Context.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    await Context.Entries.AddAsync(new Entry
                    {
                        Day = entry.Day,
                        TaskId = entry.TaskId,
                        Value = entry.Value
                    }).ConfigureAwait(false);
                }

                return true;
            }

            return false;
        }
    }
}
