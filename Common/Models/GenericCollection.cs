using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class GenericCollection<T>
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ICollection<T> Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Count => Items?.Count ?? 0;
    }
}
