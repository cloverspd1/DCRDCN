namespace BEL.DCRDCNWorkflow.Models.DCR
{
    using System;

    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;

    /// <summary>
    /// Discount Requisition Section
    /// </summary>
    [DataContract, Serializable]
    public class QASection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QASection"/> class.
        /// </summary>
        public QASection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QASection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public QASection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true) };
                this.SectionName = DCRSectionName.QASECTION;
                this.Files = new List<FileDetails>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
            }
        }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SendBackTo { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the form belong to.
        /// </summary>
        /// <value>
        /// The form belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FormBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the application belong to.
        /// </summary>
        /// <value>
        /// The application belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ApplicationBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the list details.
        /// </summary>
        /// <value>
        /// The list details.
        /// </value>
        [DataMember, IsListColumn(false)]
        public List<ListItemDetail> ListDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [DataMember, IsListColumn(false)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ButtonActionStatus ActionStatus { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverDetails(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public ApplicationStatus CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }

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
        [DataMember]
        public string FNLQATestReport { get; set; }

        /// <summary>
        /// Gets or sets the Performance Test Applicable.
        /// </summary>
        /// <value>
        /// The Performance Test Applicable.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string PerformanceTestApplicable { get; set; }

        /// <summary>
        /// Gets or sets the DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool QADCRToBeConsidered { get; set; }

        /// <summary>
        /// Gets or sets the QA Effect of change on quality norms.
        /// </summary>
        /// <value>
        /// The QA Effect of change on quality norms.
        /// </value>
        [DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 character")]
        public string QAEffectOfChangeOnQualityNorms { get; set; }

        /// <summary>
        /// Gets or sets the Overall Comment.
        /// </summary>
        /// <value>
        /// The Overall Comment.
        /// </value>
        [DataMember]
        public string OverallComment { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }
    }
}
