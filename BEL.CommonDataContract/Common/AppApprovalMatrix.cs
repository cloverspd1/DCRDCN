namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Application Approval Matrix
    /// </summary>
    [DataContract, Serializable]
    public class AppApprovalMatrix
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [DataMember]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        [DataMember]
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the active section.
        /// </summary>
        /// <value>
        /// The active section.
        /// </value>
        [DataMember]
        public string ActiveSection { get; set; }

        /// <summary>
        /// Gets or sets the active section.
        /// </summary>
        /// <value>
        /// The active section.
        /// </value>
        [DataMember]
        public string HiddenSection { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [DataMember]
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the req identifier.
        /// </summary>
        /// <value>
        /// The req identifier.
        /// </value>
        [DataMember, FieldColumnName("RequestID", true)]
        public int RequestID { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [DataMember]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the name of the approver.
        /// </summary>
        /// <value>
        /// The name of the approver.
        /// </value>
        [DataMember]
        public string ApproverName { get; set; }

        /// <summary>
        /// Gets or sets the approval status.
        /// </summary>
        /// <value>
        /// The approval status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the assign date.
        /// </summary>
        /// <value>
        /// The assign date.
        /// </value>
        [DataMember]
        public DateTime? AssignDate { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the approval date.
        /// </summary>
        /// <value>
        /// The approval date.
        /// </value>
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic approve.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is automatic approve; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoApprove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is escalate.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is escalate; otherwise, <c>false</c>.
        /// </value>
        public bool IsEscalate { get; set; }
       
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        public string Division { get; set; }

        /// <summary>
        /// Gets or sets the sub division.
        /// </summary>
        /// <value>
        /// The sub division.
        /// </value>
        public string SubDivision { get; set; }

        /// <summary>
        /// Gets or sets the type of the form.
        /// </summary>
        /// <value>
        /// The type of the form.
        /// </value>
        [DataMember]
        public string FormType { get; set; }
    }
}
