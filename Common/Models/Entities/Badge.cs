using System;
using System.Runtime.Serialization;

namespace Common.Models.Entities
{
    /// <summary>
    /// Badge.
    /// </summary>
    [DataContract]
    public class Badge
    {
        /// <summary>
        /// Badge id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Badge name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
