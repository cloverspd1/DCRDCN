﻿namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Activity Log Section
    /// </summary>
    [DataContract, Serializable]
    public class ActivityLogSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogSection"/> class.
        /// </summary>
        public ActivityLogSection()
        {
            this.ActivityLogs = new List<ActivityLog>();
            this.ListDetails = new List<ListItemDetail>();
            this.SectionName = SectionNameConstant.ACTIVITYLOG;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogSection"/> class.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        public ActivityLogSection(string listName)
        {
            this.ActivityLogs = new List<ActivityLog>();
            this.ListDetails = new List<ListItemDetail>();
            this.ListDetails.Add(new ListItemDetail(listName, false));
            this.SectionName = SectionNameConstant.ACTIVITYLOG;
        }

        /// <summary>
        /// Gets or sets the activity logs.
        /// </summary>
        /// <value>
        /// The activity logs.
        /// </value>
        [DataMember]
        public List<ActivityLog> ActivityLogs = new List<ActivityLog>();

        /// <summary>
        /// Gets or sets the list details.
        /// </summary>
        /// <value>
        /// The list details.
        /// </value>
        [DataMember]
        public List<ListItemDetail> ListDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        [DataMember]
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the form belong to.
        /// </summary>
        /// <value>
        /// The form belong to.
        /// </value>
        [DataMember]
        public string FormBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the application belong to.
        /// </summary>
        /// <value>
        /// The application belong to.
        /// </value>
        [DataMember]
        public string ApplicationBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember]
        public ButtonActionStatus ActionStatus { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }
    }
}
