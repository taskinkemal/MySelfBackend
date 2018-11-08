using System;
using System.Runtime.Serialization;


namespace Common.Models
{
    /// <summary>
    /// Entry
    /// </summary>
    [DataContract]
    public class Entry
    {
        /// <summary>
        /// Date of the entry
        /// </summary>
        [DataMember]
        public int Day { get; set; }

        /// <summary>
        /// Task id
        /// </summary>
        [DataMember]
        public int TaskId { get; set; }

        /// <summary>
        /// Value; only 0 or 1 for boolean tasks.
        /// </summary>
        [DataMember]
        public int Value { get; set; }
        
        /// <summary>
        /// Insert / last update date
        /// </summary>
        [DataMember]
        public DateTime ModificationDate { get; set; }
    }
}
