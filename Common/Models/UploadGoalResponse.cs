using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Models.Entities;

namespace Common.Models
{
    /// <summary>
    /// Upload goal response.
    /// </summary>
    [DataContract]
    public class UploadGoalResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Common.Models.UploadGoalResponse"/> class.
        /// </summary>
        public UploadGoalResponse()
        {
            NewBadges = new List<UserBadge>();
        }

        /// <summary>
        /// Gets or sets the goal identifier.
        /// </summary>
        /// <value>The goal identifier.</value>
        [DataMember]
        public int GoalId { get; set; }

        /// <summary>
        /// Total user score after upload
        /// </summary>
        /// <value>The score.</value>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the new badges.
        /// </summary>
        /// <value>The new badges.</value>
        [DataMember]
        public List<UserBadge> NewBadges { get; set; }
    }
}
