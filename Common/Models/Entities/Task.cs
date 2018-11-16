using System;
using System.Runtime.Serialization;


namespace Common.Models.Entities
{
    /// <summary>
    /// Task
    /// </summary>
    [DataContract]
    public class Task
    {
        /// <summary>
        /// Task id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Task label
        /// </summary>
        [DataMember]
        public string Label { get; set; }

        /// <summary>
        /// Data type (0: boolean, 1: numeric)
        /// </summary>
        [DataMember]
        public int DataType { get; set; }

        /// <summary>
        /// Task unit (for numeric tasks)
        /// </summary>
        [DataMember]
        public string Unit { get; set; }

        /// <summary>
        /// Whether the task has a goal set or not.
        /// </summary>
        [DataMember]
        public bool HasGoal { get; set; }

        /// <summary>
        /// 1: exactly, 2: or more, 3: or less
        /// </summary>
        [DataMember]
        public int GoalMinMax { get; set; }

        /// <summary>
        /// The goal set for the task
        /// </summary>
        [DataMember]
        public int Goal { get; set; }

        /// <summary>
        /// 1: day, 2: week, 3: month
        /// </summary>
        [DataMember]
        public int GoalTimeFrame { get; set; }

        /// <summary>
        /// Task owner
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Task status (0: deleted, 1: active)
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// Insert / last update date
        /// </summary>
        [DataMember]
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// 1: Call duration, 2: App usage, 3: Wifi network
        /// </summary>
        [DataMember]
        public int? AutomationType { get; set; }

        /// <summary>
        /// Automation variable (app or wifi identifier)
        /// </summary>
        [DataMember]
        public string AutomationVar { get; set; }
    }
}
