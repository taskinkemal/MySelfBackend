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
    }
}
