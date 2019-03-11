namespace BEL.DCRDCNWorkflow.Models.DCR
{
    using System;

    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;
    using BEL.DCRDCNWorkflow.Models.Master;


    /// <summary>
    /// Discount Requisition Section
    /// </summary>
    [DataContract, Serializable]
    public class DCRProcessInchargeSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Approver1Section"/> class.
        /// </summary>
        public DCRProcessInchargeSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Approver1Section"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public DCRProcessInchargeSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true) };
                this.SectionName = DCRSectionName.DCRPROCESSINCHARGESECTION;
                this.Files = new List<FileDetails>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new ApproverMaster());
            }
        }

        /// <summary>
        /// Gets or sets the master data.
        /// </summary>
        /// <value>
        /// The master data.
        /// </value>
        [DataMember, IsListColumn(false), ContainsMasterData(true)]
        public List<IMaster> MasterData { get; set; }

        /// <summary>
        /// Gets or sets the DCRDCN Incharge Comments.
        /// </summary>
        /// <value>
        /// The DCRDCN Incharge Comments.
        /// </value>
        [DataMember]
        public string RejectedComment { get; set; }

        /// <summary>
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, IsPerson(true, true), IsViewer]
        public string DesignEngineer { get; set; }

        /// <summary>
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, IsPerson(true, true, true), IsViewer, FieldColumnName("DesignEngineer")]
        public string DesignEngineerName { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Engineer.
        ///// </summary>
        ///// <value>
        ///// The Design Engineer.
        ///// </value>
        //[DataMember, IsPerson(true, false), IsViewer]
        //public string  DesignEngineerIncharge { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Engineer.
        ///// </summary>
        ///// <value>
        ///// The Design Engineer.
        ///// </value>
        //[DataMember, IsPerson(true, false, true), IsViewer, FieldColumnName("DesignEngineerIncharge")]
        //public string DesignEngineerInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the D&D HOD DISPOSAL.
        /// </summary>
        /// <value>
        /// The D&D HOD DISPOSAL.
        /// </value>
        [DataMember, Required]
        public string DDHODDISPOSAL { get; set; }

        /// <summary>
        /// Gets or sets the If Not Considered .
        /// </summary>
        /// <value>
        /// The If Not Considered .
        /// </value>
        [DataMember, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string IfNotConsidered { get; set; }

        /// <summary>
        /// Gets or sets the rework comments.
        /// </summary>
        /// <value>
        /// The rework comments.
        /// </value>
        [DataMember, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string ReworkComments { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Approval Required .
        /// </summary>
        /// <value>
        /// The Marketing Approval Required .
        /// </value>
        [DataMember]
        public bool MarketingApprovalRequired { get; set; }

        ///// <summary>
        ///// Gets or sets the product name.
        ///// </summary>
        ///// <value>
        ///// The product name.
        ///// </value>
        //[DataMember]
        //public string ProductName { get; set; }

        ///// <summary>
        ///// Gets or sets the product category.
        ///// </summary>
        ///// <value>
        ///// The product categoty.
        ///// </value>
        //[DataMember]
        //public string ProductCategory { get; set; }

        ///// <summary>
        ///// Gets or sets the visit location.
        ///// </summary>
        ///// <value>
        ///// The visit location.
        ///// </value>
        //[DataMember]
        //public string BusinessUnit { get; set; }

        ///// <summary>
        ///// Gets or sets the department.
        ///// </summary>
        ///// <value>
        ///// The department.
        ///// </value>
        //[DataMember, Required]
        //public string Department { get; set; }

        ///// <summary>
        ///// Gets or sets the Design change proposed due to.
        ///// </summary>
        ///// <value>
        ///// The Design change proposed due to.
        ///// </value>
        //[DataMember, Required]
        //public string Designchangeproposeddueto { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Change Proposed Other.
        ///// </summary>
        ///// <value>
        ///// The Design Change Proposed Other.
        ///// </value>
        //[DataMember]
        //public string DesignChangeProposedOther { get; set; }

        ///// <summary>
        ///// Gets or sets the tour to date.
        ///// </summary>
        ///// <value>
        ///// The tour to date.
        ///// </value>
        //[DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters")]
        //public string DescriptionOfDesignChangePropose { get; set; }

        ///// <summary>
        ///// Gets or sets the division code.
        ///// </summary>
        ///// <value>
        ///// The division code.
        ///// </value>
        //[DataMember]
        //public DateTime? DesignChangeRequiredByDate { get; set; }   //datetime or string?

        ///// <summary>
        ///// Gets or sets the division code.
        ///// </summary>
        ///// <value>
        ///// The division code.
        ///// </value>
        //[DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters")]
        //public string Expectedresultsifdesignchangeisc { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Change Proposed Attachment.
        ///// </summary>
        ///// <value>
        ///// The Design Change Proposed Attachment.
        ///// </value>
        //[DataMember, IsFile(true)]
        //public List<FileDetails> DesignChangeProposedAttachment { get; set; }

        ///// <summary>
        ///// Gets or sets the Expected Results If Design Change Attachment.
        ///// </summary>
        ///// <value>
        ///// The Expected Results If Design Change Attachment.
        ///// </value>
        //[DataMember, IsFile(true)]
        //public List<FileDetails> ExpectedResultsIfDesignChangeAttachment { get; set; }

        ///// <summary>
        ///// Gets or sets the Old DCR No.
        ///// </summary>
        ///// <value>
        ///// The Old DCR No.
        ///// </value>
        //[DataMember]
        //public string OldDCRNo { get; set; }

        ///// <summary>
        ///// Gets or sets the Old DCR Created Date.
        ///// </summary>
        ///// <value>
        ///// The Old DCR Created Date.
        ///// </value>
        //[DataMember]
        //public DateTime? OldDCRCreatedDate { get; set; }

        ///// <summary>
        ///// Gets or sets the Old DCR Rejected Date.
        ///// </summary>
        ///// <value>
        ///// The Old DCR Rejected Date.
        ///// </value>
        //[DataMember]
        //public DateTime? OldDCRRejectedDate { get; set; }

        ///// <summary>
        ///// Gets or sets the request date.
        ///// </summary>
        ///// <value>
        ///// The request date.
        ///// </value>
        //[DataMember, IsPerson(true, false)]
        //public string ProposedBy { get; set; }

        ///// <summary>
        ///// Gets or sets the request date.
        ///// </summary>
        ///// <value>
        ///// The request date.
        ///// </value>
        //[DataMember, DataType(DataType.Date)]
        //public DateTime? RequestDate { get; set; }

        /////// <summary>
        /////// Gets or sets the requested by.
        /////// </summary>
        /////// <value>
        /////// The requested by.
        /////// </value>
        ////[DataMember, IsPerson(true, false)]
        ////public string RequestBy { get; set; }

        ///// <summary>
        ///// Gets or sets the reference no.
        ///// </summary>
        ///// <value>
        ///// The reference no.
        ///// </value>
        //[DataMember]
        //public string DCRNo { get; set; }

        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        /// <value>
        /// The workflow status.
        /// </value>
        [DataMember]
        public string WorkflowStatus { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
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
        /// Gets or sets the Old DCR Rejected Date.
        /// </summary>
        /// <value>
        /// The Old DCR Rejected Date.
        /// </value>
        [DataMember, DataType(DataType.Date)]
        public DateTime? OldDCRRejectedDate { get; set; }

        /// <summary>
        /// Gets or sets the visit location.
        /// </summary>
        /// <value>
        /// The visit location.
        /// </value>
        [DataMember]
        public string BusinessUnit { get; set; }

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
        /// Gets or sets the FNLDCR attachment.
        /// </summary>
        /// <value>
        /// The FNLDCR attachment.
        /// </value>
        [DataMember]
        public string FNLDCRAttachment { get; set; }

        /// <summary>
        /// Gets or sets the is rework required.
        /// </summary>
        /// <value>
        /// The is rework required.
        /// </value>
        [DataMember, IsListColumn(false)]
        public bool? IsReworkRequired { get; set; }

        /// <summary>
        /// Gets or sets the target date of implementation.
        /// </summary>
        /// <value>
        /// The target date of implementation.
        /// </value>
        [DataMember, Required]
        public DateTime? TargetDateOfImplementation { get; set; }
    }
}
