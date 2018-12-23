using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common.Models.Entities
{
    /// <summary>
    /// User
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Initializes user entity.
        /// </summary>
        public User()
        {
            Badges = new List<UserBadge>();
        }

        /// <summary>
        /// User unique id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// User first name
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// User last name
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// User picture Url
        /// </summary>
        [DataMember]
        public string PictureUrl { get; set; }

        /// <summary>
        /// User gamification score
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// User gamification badges
        /// </summary>
        [DataMember]
        public List<UserBadge> Badges { get; set; }
    }
}
