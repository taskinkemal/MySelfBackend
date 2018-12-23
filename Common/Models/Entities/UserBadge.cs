using System;
using System.Runtime.Serialization;

namespace Common.Models.Entities
{
    /// <summary>
    /// User badge.
    /// </summary>
    [DataContract]
    public class UserBadge
    {
        /// <summary>
        /// User id
        /// </summary>
        public int UserId { get; set; }

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
        /// Insert date
        /// </summary>
        [DataMember]
        public DateTime ModificationDate { get; set; }
    }
}
