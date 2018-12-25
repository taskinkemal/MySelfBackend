using System;
using System.Runtime.Serialization;

namespace Common.Models.Entities
{
    /// <summary>
    /// Achieved goal.
    /// </summary>
    [DataContract]
    public class AchievedGoal
    {
        /// <summary>
        /// Task id
        /// </summary>
        [DataMember]
        public int TaskId { get; set; }

        /// <summary>
        /// The day that the goal is achieved
        /// </summary>
        [DataMember]
        public int Day { get; set; }
    }
}
