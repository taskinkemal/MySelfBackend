using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Models.Entities;

namespace Common.Models
{
    /// <summary>
    /// Upload entry response.
    /// </summary>
    [DataContract]
    public class UploadEntryResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Common.Models.UploadEntryResponse"/> class.
        /// </summary>
        public UploadEntryResponse()
        {
            NewBadges = new List<UserBadge>();
        }

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
