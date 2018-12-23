using System;
namespace Common.Extensions
{
    /// <summary>
    /// Date time extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the day.
        /// </summary>
        /// <returns>The day.</returns>
        /// <param name="date">Date.</param>
        public static int GetDay(this DateTime date)
        {
            return (int)(date - new DateTime(2018, 1, 1)).TotalDays;
        }
    }
}
