using System.Collections.Generic;
using Common.Interfaces;
using Common.Models;


namespace DataLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntryManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns></returns>
        System.Threading.Tasks.Task<List<Entry>> GetEntries(int userId, int start, int end);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> AddOrUpdateEntry(int userId, Entry entry);
    }
}
