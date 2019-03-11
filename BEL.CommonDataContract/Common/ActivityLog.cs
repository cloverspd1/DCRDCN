namespace BEL.CommonDataContract
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Activity Log
    /// </summary>
    [DataContract, Serializable]
    public class ActivityLog
    {
        /// <summary>
        /// Gets or sets the no.
        /// </summary>
        /// <value>
        /// The no.
        /// </value>
        [DataMember]
        public int No { get; set; }
        
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>
        /// The activity.
        /// </value>
        [DataMember]
        public string Activity { get; set; }

        /// <summary>
        /// Gets or sets the changes.
        /// </summary>
        /// <value>
        /// The changes.
        /// </value>
        [DataMember]
        public string Changes { get; set; }
        
        /// <summary>
        /// Gets or sets the performed by.
        /// </summary>
        /// <value>
        /// The performed by.
        /// </value>
        [DataMember]
        public string PerformedBy { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        [DataMember]
        public string SectionName { get; set; }
        
        /// <summary>
        /// Gets or sets the performed on.
        /// </summary>
        /// <value>
        /// The performed on.
        /// </value>
        [DataMember]
        public DateTime? PerformedOn { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>
        /// The created on.
        /// </value>
        [DataMember]
        public DateTime? Created { get; set; }
    }
}
