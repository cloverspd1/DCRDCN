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
    public class DCRDetailSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        public DCRDetailSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public DCRDetailSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true) };
                this.SectionName = DCRSectionName.DCRDETAILSECTION;
                this.Files = new List<FileDetails>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                ////Add All Master Object which required for this Section.
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new ApproverMaster());
                this.MasterData.Add(new BusinessUnitMaster());
                this.MasterData.Add(new DivisionMaster());
                this.MasterData.Add(new ProductCategoryMaster());
                this.MasterData.Add(new DesignchangeproposedMaster());
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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the visit location.
        /// </summary>
        /// <value>
        /// The visit location.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        [DataMember, FieldColumnName("Department")]
        public string RequestDepartment { get; set; }


        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string Division { get; set; }

        /// <summary>
        /// Gets or sets the product category.
        /// </summary>
        /// <value>
        /// The product categoty.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        /// <value>
        /// The product name.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters"), RequiredOnDraft]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the Design change proposed due to.
        /// </summary>
        /// <value>
        /// The Design change proposed due to.
        /// </value>
        [DataMember, Required]
        public string Designchangeproposeddueto { get; set; }

        /// <summary>
        /// Gets or sets the Design Change Proposed Other.
        /// </summary>
        /// <value>
        /// The Design Change Proposed Other.
        /// </value>
        [DataMember]
        public string DesignChangeProposedOther { get; set; }

        /// <summary>
        /// Gets or sets the tour to date.
        /// </summary>
        /// <value>
        /// The tour to date.
        /// </value>
        [DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters"), FieldColumnName("DescriptionOfDesignChangePropose")]
        public string DescriptionOfDesignChangePropose { get; set; }

        /// <summary>
        /// Gets or sets the division code.
        /// </summary>
        /// <value>
        /// The division code.
        /// </value>
        [DataMember, FieldColumnName("DesignChangeRequiredByDate")]
        public DateTime? DesignChangeRequiredByDate { get; set; }   //datetime or string?

        /// <summary>
        /// Gets or sets the division code.
        /// </summary>
        /// <value>
        /// The division code.
        /// </value>
        [DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters")]
        public string Expectedresultsifdesignchangeisc { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string FNLDesignChangeAttachment { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string FNLExpectedResultsAttachment { get; set; }

        /// <summary>
        /// Gets or sets the Old DCR No.
        /// </summary>
        /// <value>
        /// The Old DCR No.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string DCRInchargeNavigator { get; set; }

        /// <summary>
        /// Gets or sets the Old DCR No.
        /// </summary>
        /// <value>
        /// The Old DCR No.
        /// </value>
        [DataMember]
        public string OldDCRNo { get; set; }

        /// <summary>
        /// Old DCR ID
        /// </summary>
        [DataMember, IsListColumn(false)]
        public int OldDCRId { get; set; }

        /// <summary>
        /// Gets or sets the Old DCR Created Date.
        /// </summary>
        /// <value>
        /// The Old DCR Created Date.
        /// </value>
        [DataMember]
        public DateTime? OldDCRCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the Old DCR Rejected Date.
        /// </summary>
        /// <value>
        /// The Old DCR Rejected Date.
        /// </value>
        [DataMember]
        public DateTime? OldDCRRejectedDate { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string ProposedBy { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("ProposedBy"), IsViewer]
        public string ProposedByName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the requested by.
        /// </summary>
        /// <value>
        /// The requested by.
        /// </value>
        [DataMember]
        public bool IsDCRRetrieved { get; set; }

        /// <summary>
        /// Gets or sets the reference no.
        /// </summary>
        /// <value>
        /// The reference no.
        /// </value>
        [DataMember]
        public string DCRNo { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        /// <value>
        /// The workflow status.
        /// </value>
        [DataMember]
        public string WorkflowStatus { get; set; }


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
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }

        /// <summary>
        /// Gets or sets the design engineer incharge.
        /// </summary>
        /// <value>
        /// The design engineer incharge.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string DesignEngineerIncharge { get; set; }

        /// <summary>
        /// Gets or sets the design engineer incharge.
        /// </summary>
        /// <value>
        /// The design engineer incharge.
        /// </value>
        [DataMember, IsPerson(true, true), IsViewer]
        public string DesignEngineer { get; set; }

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
        /// Gets or sets the If Not Considered .
        /// </summary>
        /// <value>
        /// The If Not Considered .
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

        /// <summary>
        /// Gets or sets the FNLDCR attachment.
        /// </summary>
        /// <value>
        /// The FNLDCR attachment.
        /// </value>
        [DataMember]
        public string FNLDCRAttachment { get; set; }

        /// <summary>
        /// Gets or sets the target date of implementation.
        /// </summary>
        /// <value>
        /// The target date of implementation.
        /// </value>
        [DataMember]
        public DateTime? TargetDateOfImplementation { get; set; }

        /// <summary>
        /// Gets or sets the ConsiderRework.
        /// </summary>
        /// <value>
        /// The ConsiderRework.
        /// </value>
        [DataMember]
        public string ConsiderRework { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
        public string FinalDesignEngineer { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("FinalDesignEngineer"), IsViewer]
        public string FinalDesignEngineerName { get; set; }

    }
}
