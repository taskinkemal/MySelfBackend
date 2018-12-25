using System;

namespace Common.Extensions
{
    /// <summary>
    /// Integer extensions.
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <returns>The date.</returns>
        /// <param name="value">Value.</param>
        public static DateTime GetDate(this int value)
        {
            return new DateTime(2018, 1, 1).AddDays(value);
        }
    }
}
