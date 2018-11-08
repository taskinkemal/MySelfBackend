using System.Collections.Generic;
using Common.Models;


namespace Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        public static GenericCollection<T> ToCollection<T>(this ICollection<T> sequence)
        {
            return new GenericCollection<T>
            {
                Items = sequence
            };
        }
    }
}
