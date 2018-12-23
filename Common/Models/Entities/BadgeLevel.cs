using System;
using System.Runtime.Serialization;

namespace Common.Models.Entities
{
    /// <summary>
    /// Badge level.
    /// </summary>
    [DataContract]
    public class BadgeLevel
    {
        /// <summary>
        /// Badge id
        /// </summary>
        [DataMember]
        public int BadgeId { get; set; }

        /// <summary>
        /// Badge level
        /// </summary>
        [DataMember]
        public int Level { get; set; }

        /// <summary>
        /// Level name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
