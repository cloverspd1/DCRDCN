namespace BEL.DCRDCNWorkflow.Models.DCN
{
    using System;
    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;

    /// <summary>
    /// DCR Process Incharge Section
    /// </summary>
    [DataContract, Serializable]
    public class DCRProcessInchargeSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternateApprover1Section"/> class.
        /// </summary>
        public DCRProcessInchargeSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternateApprover1Section"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public DCRProcessInchargeSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCNLIST, true) };
                this.SectionName = DCNSectionName.DCRPROCESSINCHRGESECTION;
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
            }
        }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        ///// <summary>
        ///// Gets or sets the Status.
        ///// </summary>
        ///// <value>
        ///// The Status.
        ///// </value>
        //[DataMember, Required, FieldColumnName("DCRProcessInchargeComment")]
        //public string DCRProcessInchargeComment { get; set; }

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
        [DataMember, IsListColumn(false), IsApproverDetails(true, DCRDCNListNames.DCNAPPAPPROVALMATRIX)]
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
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCNAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        [DataMember, Required]
        public string DCRProcessICApproveRework { get; set; }

        [DataMember]
        public string DCRDCNAttachment { get; set; }

        
    }
}
