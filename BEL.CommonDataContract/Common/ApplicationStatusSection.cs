namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Activity Log Section
    /// </summary>
    [DataContract, Serializable]
    public class ApplicationStatusSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStatusSection"/> class.
        /// </summary>
        public ApplicationStatusSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStatusSection" /> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public ApplicationStatusSection(bool isSet)
        {
            if (isSet)
            {
                this.SectionName = SectionNameConstant.APPLICATIONSTATUS;
                this.ListDetails = new List<ListItemDetail>();
            }
        }

        /// <summary>
        /// Gets or sets the activity logs.
        /// </summary>
        /// <value>
        /// The activity logs.
        /// </value>
        [DataMember]
        public List<ApplicationStatus> ApplicationStatusList = new List<ApplicationStatus>();

        /// <summary>
        /// Gets or sets the list details.
        /// </summary>
        /// <value>
        /// The list details.
        /// </value>
        public List<ListItemDetail> ListDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the form belong to.
        /// </summary>
        /// <value>
        /// The form belong to.
        /// </value>
        public string FormBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the application belong to.
        /// </summary>
        /// <value>
        /// The application belong to.
        /// </value>
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
