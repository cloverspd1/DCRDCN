namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// Application Status details
    /// </summary>
    [DataContract, Serializable]
    public class ApplicationStatus
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
        public string HiddenSection { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [DataMember]
        public string Levels { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [DataMember]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        /// <value>
        /// The days.
        /// </value>
        [DataMember, FieldColumnName("Days"), DisplayName("Escalation / Auto Approval Days")]
        public double Days { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic approve.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is automatic approve; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsAutoApproval { get; set; }

        /// <summary>
        /// Gets or sets the fill by role.
        /// </summary>
        /// <value>
        /// The fill by role.
        /// </value>
        [DataMember]
        public string FillByRole { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string Division { get; set; }

        /// <summary>
        /// Gets or sets the sub division.
        /// </summary>
        /// <value>
        /// The sub division.
        /// </value>
        [DataMember]
        public string SubDivision { get; set; }

        /// <summary>
        /// Gets or sets the name of the approver.
        /// </summary>
        /// <value>
        /// The name of the approver.
        /// </value>
        [DataMember, IsPerson(true, true)]
        public string Approver { get; set; }

        /// <summary>
        /// Gets or sets the name of the approver.
        /// </summary>
        /// <value>
        /// The name of the approver.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("Approver")]
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
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        [DataMember]
        public string Comments { get; set; }

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
        [DataMember]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the approval date.
        /// </summary>
        /// <value>
        /// The approval date.
        /// </value>
        [DataMember]
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is escalate.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is escalate; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsEscalate { get; set; }

        /// <summary>
        /// Is Reminder Flag
        /// </summary>
        [DataMember]
        public bool IsReminder { get; set; }

        /// <summary>
        /// Gets or sets the req identifier.
        /// </summary>
        /// <value>
        /// The req identifier.
        /// </value>
        [DataMember, FieldColumnName("RequestID", true, false, "ID")]
        public int RequestID { get; set; }

        /// <summary>
        /// Gets or sets the approve by.
        /// </summary>
        /// <value>
        /// The approve by.
        /// </value>
        [DataMember, IsPerson(true, false)]
        public string ApproveBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is optional; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsOptional { get; set; }

        /// <summary>
        /// Gets or sets the type of the form.
        /// </summary>
        /// <value>
        /// The type of the form.
        /// </value>
        [DataMember]
        public string FormType { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        [DataMember, IsFile(true)]
        public List<FileDetails> Files { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FileNameList { get; set; }

        /// <summary>
        /// Gets or sets the reason for change.
        /// </summary>
        /// <value>
        /// The reason for change.
        /// </value>
        [DataMember]
        public string ReasonForChange { get; set; }

        /// <summary>
        /// Gets or sets the reason for delay.
        /// </summary>
        /// <value>
        /// The reason for delay.
        /// </value>
        [DataMember]
        public string ReasonForDelay { get; set; }
    }
}
