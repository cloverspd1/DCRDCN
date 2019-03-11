namespace BEL.DCRDCNWorkflow.Models
{
    using System;
    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;
    using BEL.DCRDCNWorkflow.Models.Master;
    using BEL.DCRDCNWorkflow.Models.DCR;

    /// <summary>
    /// Discount Requisition Section
    /// </summary>
    [DataContract, Serializable]
    public class DCRAdminDetailSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        public DCRAdminDetailSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DCRSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public DCRAdminDetailSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true) };
                this.SectionName = DCRSectionName.DCRDETAILADMINSECTION;
                this.Files = new List<FileDetails>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                this.VendorDCRList = new List<ITrans>();
                ////Add All Master Object which required for this Section.
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new ApproverMaster());
                this.MasterData.Add(new BusinessUnitMaster());
                this.MasterData.Add(new DivisionMaster());
                this.MasterData.Add(new ProductCategoryMaster());
                this.MasterData.Add(new DesignchangeproposedMaster());
                this.MasterData.Add(new EffectOnCostMaster());
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
        /// Gets or sets the Vendor DCR.
        /// </summary>
        /// <value>
        /// The Vendor DCR
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, DCRDCNListNames.VENDORDCRLIST, typeof(VendorDCR))]
        public List<ITrans> VendorDCRList { get; set; }

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
        [DataMember, Required, NotSavedColumnAttribute(true), RequiredOnDraft]
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
        [DataMember, Required, NotSavedColumnAttribute(true), RequiredOnDraft]
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
        public DateTime? DesignChangeRequiredByDate { get; set; }

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
        [DataMember, IsPerson(true, false)]
        public string ProposedBy { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("ProposedBy")]
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
        /// Gets or sets the reference no.
        /// </summary>
        /// <value>
        /// The reference no.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the DCR Incharge.
        /// </summary>
        /// <value>
        /// The DCR Incharge.
        /// </value>
        [DataMember, Required, IsPerson(true, false), NotSavedColumnAttribute(true)]
        public string DCRIncharge { get; set; }

        /// <summary>
        /// Gets or sets the DCR Incharge.
        /// </summary>
        /// <value>
        /// The DCR Incharge.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("DCRIncharge")]
        public string DCRInchargeName { get; set; }

        // <summary>
        /// Gets or sets the Comment.
        /// </summary>
        /// <value>
        /// The Comment.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DCRInchargeNavigatorComment { get; set; }

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
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, IsPerson(true, true), NotSavedColumnAttribute(true)]
        public string DesignEngineer { get; set; }

        /// <summary>
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DesignEngineerComment { get; set; }

        /// <summary>
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string DesignEngineerIncharge { get; set; }

        /// <summary>
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, IsPerson(true, false, true), IsViewer, FieldColumnName("DesignEngineerIncharge")]
        public string DesignEngineerInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        /// <value>
        /// The Comment.
        /// </value>
        [DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters"), IsListColumn(false)]
        public string DesignEngineerIComment { get; set; }

        /// <summary>
        /// Gets or sets the Design Engineer.
        /// </summary>
        /// <value>
        /// The Design Engineer.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("DesignEngineer")]
        public string DesignEngineerName { get; set; }

        /// <summary>
        /// Gets or sets the D&D HOD DISPOSAL.
        /// </summary>
        /// <value>
        /// The D&D HOD DISPOSAL.
        /// </value>
        [DataMember, Required, NotSavedColumnAttribute(true)]
        public string DDHODDISPOSAL { get; set; }

        /// <summary>
        /// Gets or sets the D&D HOD DISPOSAL.
        /// </summary>
        /// <value>
        /// The D&D HOD DISPOSAL.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DCRProcessIComment { get; set; }

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
        /// Gets or sets the DCRDCN Incharge Comments.
        /// </summary>
        /// <value>
        /// The DCRDCN Incharge Comments.
        /// </value>
        [DataMember]
        public string RejectedComment { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Approval Required .
        /// </summary>
        /// <value>
        /// The Marketing Approval Required .
        /// </value>
        [DataMember, NotSavedColumnAttribute(true)]
        public bool MarketingApprovalRequired { get; set; }

        /// <summary>
        /// Gets or sets the Details of changes in components and assembly.
        /// </summary>
        /// <value>
        /// The Details of changes in components and assembly.
        /// </value>
        [DataMember, Required, FieldColumnName("Detailsofchangesincomponentsanda"), MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters")]
        public string Detailsofchangesincomponents { get; set; }

        /// <summary>
        /// Gets or sets the Details of changes in tools, jigs and fixtures.
        /// </summary>
        /// <value>
        /// The Details of changes in tools, jigs and fixtures.
        /// </value>
        [DataMember, Required, FieldColumnName("Detailsofchangesintoolsjigsandfi"), MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters")]
        public string Detailsofchangesintoolsjigs { get; set; }

        /// <summary>
        /// Gets or sets the Effect on performance .
        /// </summary>
        /// <value>
        /// The Effect on performance .
        /// </value>
        [DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 characters")]
        public string EffectOnPerformance { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string FNLDEAttachment { get; set; }

        /// <summary>
        /// Gets or sets the time for receipt of samples.
        /// </summary>
        /// <value>
        /// The time for receipt of samples.
        /// </value>
        [DataMember, Required]
        public DateTime? DateForReceiptOfSamples { get; set; }

        /// <summary>
        /// Gets or sets the time for completion of testing.
        /// </summary>
        /// <value>
        /// The time for completion of testing.
        /// </value>
        [DataMember, Required]
        public DateTime? DateForCompletionOfTesting { get; set; }

        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SendBackTo { get; set; }

        /// <summary>
        /// Gets or sets the SCM User.
        /// </summary>
        /// <value>
        /// The SCM User.
        /// </value>
        [DataMember, IsPerson(true, true, false), Required, RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string SCMUser { get; set; }

        /// <summary>
        /// Gets or sets the SCM User.
        /// </summary>
        /// <value>
        /// The SCM User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("SCMUser")]
        public string SCMUserName { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        /// <value>
        /// The Comment.
        /// </value>
        [DataMember, MaxLength(500, ErrorMessage = "this field should not be more then 500 characters"), FieldColumnName("SCMUserComment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        /// <value>
        /// The Comment.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters"), IsListColumn(false)]
        public string SCMIComment { get; set; }

        /// <summary>
        /// Gets or sets the Effect on cost of part and product.
        /// </summary>
        /// <value>
        /// The Effect on cost of part and product.
        /// </value>
        [DataMember, Required]
        public string EffectOnCostOfPartAndProduct { get; set; }

        /// <summary>
        /// Gets or sets the Cost reduced / Increased by Rs.
        /// </summary>
        /// <value>
        /// The Cost reduced / Increased by Rs.
        /// </value>
        [DataMember, Required]
        public double? CostReducedIncreasedByRs { get; set; }

        /// <summary>
        /// Gets or sets the Total Expected Quantity in current year.
        /// </summary>
        /// <value>
        /// The Total Expected Quantity in current year.
        /// </value>
        [DataMember, Required]
        public double? TotalExpectedQuantityInCurrentYe { get; set; }

        /// <summary>
        /// Gets or sets the Total Expected Quantity in next year.
        /// </summary>
        /// <value>
        /// The Total Expected Quantity in next year.
        /// </value>
        [DataMember, Required]
        public double? TotalExpectedQuantityInNextYear { get; set; }

        /// <summary>
        /// Gets or sets the Total Benefit / Loss in Rupees.
        /// </summary>
        /// <value>
        /// The Total Benefit / Loss in Rupees.
        /// </value>
        [DataMember, Required]
        public double? TotalBenefitLossInRupeesLakhs { get; set; }

        /// <summary>
        /// Gets or sets the Feedback from SCM .
        /// </summary>
        /// <value>
        /// The Feedback from SCM .
        /// </value>
        [DataMember, Required, MaxLength(2500, ErrorMessage = "this field should not be more then 2500 characters")]
        public string FeedbackFromSCM { get; set; }

        /// <summary>
        /// Gets or sets the QA User.
        /// </summary>
        /// <value>
        /// The QA User.
        /// </value>
        [DataMember, IsPerson(true, true, false), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string QAUser { get; set; }

        /// <summary>
        /// Gets or sets the QA User.
        /// </summary>
        /// <value>
        /// The QA User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("QAUser")]
        public string QAUserName { get; set; }

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
        [DataMember, Required, FieldColumnName("QADCRToBeConsidered")]
        public bool QADCRToBeConsidered { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string FNLQATestReport { get; set; }

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
        [DataMember, Required, MaxLength(1000, ErrorMessage = "this field should not be more then 1000 character"), FieldColumnName("QAOverallComment")]
        public string OverallComment { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string DAPMarketingUser { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DAPIComment { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("DAPMarketingUser")]
        public string DAPMarketingUserName { get; set; }

        /// <summary>
        /// Gets or sets the Marketing DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The Marketing DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool DAPTOBECONSIDERED { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Expected effect of change on sales.
        /// </summary>
        /// <value>
        /// The Marketing Expected effect of change on sales.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string DAPExpectedChange { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string FANSMarketingUser { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("FANSMarketingUser")]
        public string FANSMarketingUserName { get; set; }


        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string FANIComment { get; set; }

        /// <summary>
        /// Gets or sets the Marketing DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The Marketing DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool FANSTOBECONSIDERED { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Expected effect of change on sales.
        /// </summary>
        /// <value>
        /// The Marketing Expected effect of change on sales.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string FANSExpectedChange { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string KAPMarketingUser { get; set; }


        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string KAPIComment { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("KAPMarketingUser")]
        public string KAPMarketingUserName { get; set; }

        /// <summary>
        /// Gets or sets the Marketing DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The Marketing DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool KAPTOBECONSIDERED { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Expected effect of change on sales.
        /// </summary>
        /// <value>
        /// The Marketing Expected effect of change on sales.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string KAPExpectedChange { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string LightingMarketingUser { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string LightingIComment { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("LightingMarketingUser")]
        public string LightingMarketingUserName { get; set; }

        /// <summary>
        /// Gets or sets the Marketing DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The Marketing DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool LightingTOBECONSIDERED { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Expected effect of change on sales.
        /// </summary>
        /// <value>
        /// The Marketing Expected effect of change on sales.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string LightingExpectedChange { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string LUMMarketingUser { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string LUMIComment { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("LUMMarketingUser")]
        public string LUMMarketingUserName { get; set; }

        /// <summary>
        /// Gets or sets the Marketing DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The Marketing DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool LUMTOBECONSIDERED { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Expected effect of change on sales.
        /// </summary>
        /// <value>
        /// The Marketing Expected effect of change on sales.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string LUMExpectedChange { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true), RequiredOnDelegate, NotSavedColumnAttribute(true)]
        public string MRMarketingUser { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("MRMarketingUser")]
        public string MRMarketingUserName { get; set; }

        /// <summary>
        /// Gets or sets Marketing User.
        /// </summary>
        /// <value>
        /// The Marketing User.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string MRMarketingIComment { get; set; }

        /// <summary>
        /// Gets or sets the Marketing DCR TO BE CONSIDERED.
        /// </summary>
        /// <value>
        /// The Marketing DCR TO BE CONSIDERED.
        /// </value>
        [DataMember, Required]
        public bool MRTOBECONSIDERED { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Expected effect of change on sales.
        /// </summary>
        /// <value>
        /// The Marketing Expected effect of change on sales.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string MRExpectedChange { get; set; }

        /// <summary>
        /// Gets or sets the Consumer Care Is Feasible.
        /// </summary>
        /// <value>
        /// The Consumer Care Is Feasible.
        /// </value>
        [DataMember, Required]
        public string Feasibility { get; set; }

        /// <summary>
        /// Gets or sets the reason if not feasible.
        /// </summary>
        /// <value>
        /// The the reason if not feasible.
        /// </value>
        [DataMember, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        public string CCIfNotFeasibleGiveReason { get; set; }

        ///// <summary>
        ///// Gets or sets the DDHOD Comments.
        ///// </summary>
        ///// <value>
        ///// The DDHOD Comments.
        ///// </value>
        //[DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        //public string CommentsAfterProtoTesting { get; set; }

        /// <summary>
        /// Gets or sets the Is DCR approved.
        /// </summary>
        /// <value>
        /// The Is DCR approved.
        /// </value>
        [DataMember, Required, NotSavedColumnAttribute(true)]
        public string IsApproved { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Document Engineer.
        ///// </summary>
        ///// <value>
        ///// The Design Document Engineer.
        ///// </value>
        //[DataMember, Required, IsPerson(true, false), NotSavedColumnAttribute(true)]
        //public string DesignDocumentEngineer { get; set; }

        /// <summary>
        /// Gets or sets the final design engineer.
        /// </summary>
        /// <value>
        /// The final design engineer.
        /// </value>
        [DataMember, Required, IsPerson(true, false, false), IsViewer]
        public string FinalDesignEngineer { get; set; }

        /// <summary>
        /// Gets or sets the final design engineer.
        /// </summary>
        /// <value>
        /// The final design engineer.
        /// </value>
        [DataMember, Required, IsPerson(true, false, true), IsViewer, FieldColumnName("FinalDesignEngineer")]
        public string FinalDesignEngineerName { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Document Engineer.
        ///// </summary>
        ///// <value>
        ///// The Design Document Engineer.
        ///// </value>
        //[DataMember, IsPerson(true, false, true), FieldColumnName("DesignDocumentEngineer")]
        //public string DesignDocumentEngineerName { get; set; }

        /// <summary>
        /// Gets or sets the QA Incharge comment.
        /// </summary>
        /// <value>
        /// The QA Incharge comment.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string QAIComment { get; set; }

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
        [DataMember, Required]
        public DateTime? TargetDateOfImplementation { get; set; }
    }
}
