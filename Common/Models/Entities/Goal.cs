using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Common.Models.Entities
{
    /// <summary>
    /// Goal.
    /// </summary>
    [DataContract]
    public class Goal
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>The task identifier.</value>
        [DataMember]
        public int TaskId { get; set; }

        /// <summary>
        /// 1: exactly, 2: or more, 3: or less
        /// </summary>
        [DataMember]
        public int MinMax { get; set; }

        /// <summary>
        /// The goal set for the task
        /// </summary>
        [DataMember]
        public int Target { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        [DataMember]
        public int StartDay { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        [DataMember]
        public int EndDay { get; set; }

        /// <summary>
        /// Gets or sets the achievement status.
        /// 0: in progress, 1: achieved, 2: not achieved.
        /// </summary>
        /// <value>0: in progress, 1: achieved, 2: not achieved.</value>
        [DataMember]
        public int AchievementStatus { get; set; }

        /// <summary>
        /// Insert / last update date
        /// </summary>
        [DataMember]
        public DateTime ModificationDate { get; set; }

        [DataMember]
        [NotMapped]
        public int CurrentValue { get; set; }
    }
}
