namespace BEL.DCRDCNWorkflow.Models
{
    using System;

    using System.Runtime.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.DCN;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using BEL.DCRDCNWorkflow.Models.Common;


    /// <summary>
    /// Design Engineer Section
    /// </summary>
    [DataContract, Serializable]
    public class DCNDetailAdminSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DCNDetailAdminSection"/> class.
        /// </summary>
        public DCNDetailAdminSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DCNDetailAdminSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public DCNDetailAdminSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCNLIST, true) };
                this.SectionName = DCNSectionName.DCNDETAILADMINSECTION;
                this.Specification = new List<ITrans>();
                this.RevisedApplicable = new List<ITrans>();
                this.ApproversList = new List<ApplicationStatus>();
                this.VendorDCRList = new List<ITrans>();
                this.VendorDCNList = new List<ITrans>();
                this.CurrentApprover = new ApplicationStatus();
            }
        }

        /// <summary>
        /// Gets or sets the Vendor DCR.
        /// </summary>
        /// <value>
        /// The Vendor DCR
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, DCRDCNListNames.VENDORDCNLIST, typeof(VendorDCN))]
        public List<ITrans> VendorDCRList { get; set; }

        /// <summary>
        /// Gets or sets the Specification.
        /// </summary>
        /// <value>
        /// The Specification
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, DCRDCNListNames.SPECIFICATIONMASTER, typeof(Specification))]
        public List<ITrans> Specification { get; set; }

        /// <summary>
        /// Gets or sets the Revised Applicable.
        /// </summary>
        /// <value>
        /// The Revised Applicable.
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, DCRDCNListNames.REVISEDAPPLICABLEDOCUMENTSMASTER, typeof(RevisedApplicableDocuments))]
        public List<ITrans> RevisedApplicable { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public int DCRID { get; set; }

        /// <summary>
        /// Gets or sets the DCR No.
        /// </summary>
        /// <value>
        /// The DCR No.
        /// </value>
        [DataMember]
        public string DCRNo { get; set; }

        /// <summary>
        /// Gets or sets the Design Change Note Issued On.
        /// </summary>
        /// <value>
        /// The Design Change Note Issued On.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string DesignChangeNoteIssuedOn { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Product Name.
        /// </summary>
        /// <value>
        /// The Product Name.
        /// </value>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the Department.
        /// </summary>
        /// <value>
        /// The Department.
        /// </value>
        [DataMember]
        public string RequestDepartment { get; set; }

        /// <summary>
        /// Gets or sets the QA Incharge.
        /// </summary>
        /// <value>
        /// The QA Incharge.
        /// </value>
        [DataMember, IsPerson(true, true, false), IsViewer]
        public string QAIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("QAIncharge"), IsViewer]
        public string QAInchargeName { get; set; }

        /// <summary>
        /// Gets or sets SCM Incharge
        /// </summary>
        /// <value>
        /// The SCM Incharge
        /// </value>
        [DataMember, IsPerson(true, true, false), IsViewer]
        public string SCMIncharge { get; set; }


        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("SCMIncharge"), IsViewer]
        public string SCMInchargeName { get; set; }

        /// <summary>
        /// Gets or sets CC Incharge
        /// </summary>
        /// <value>
        /// The CC Incharge
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string CCIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("CCIncharge"), IsViewer]
        public string CCInchargeName { get; set; }

        /// <summary>
        /// Gets or sets DCR Process Incharge
        /// </summary>
        /// <value>
        /// The DCR Process Incharge
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string DCRProcessIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("DCRProcessIncharge"), IsViewer]
        public string DCRProcessInchargeName { get; set; }

        /// <summary>
        /// Gets or sets DCR Process Incharge
        /// </summary>
        /// <value>
        /// The DCR Process Incharge
        /// </value>
        [DataMember, IsPerson(true, true, false), IsViewer]
        public string DesignEngineer { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, true, true), FieldColumnName("DesignEngineer"), IsViewer]
        public string DesignEngineerName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, false), IsViewer]
        public string DAPMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("DAPMarketingIncharge"), IsViewer]
        public string DAPMarketingInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, false), IsViewer]
        public string KAPMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("KAPMarketingIncharge"), IsViewer]
        public string KAPMarketingInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, false), IsViewer]
        public string FANSMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("FANSMarketingIncharge"), IsViewer]
        public string FANSMarketingInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, false), IsViewer]
        public string LightingMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("LightingMarketingIncharge"), IsViewer]
        public string LightingMarketingInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, false), IsViewer]
        public string MRMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("MRMarketingIncharge"), IsViewer]
        public string MRMarketingInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, false), IsViewer]
        public string LUMMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("LUMMarketingIncharge"), IsViewer]
        public string LUMMarketingInchargeName { get; set; }

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
        /// Gets or sets the DCN no.
        /// </summary>
        /// <value>
        /// The DCN no.
        /// </value>
        [DataMember]
        public string DCNNo { get; set; }

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
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ButtonActionStatus ActionStatus { get; set; }

        /// <summary>
        /// Gets or sets the current approver.
        /// </summary>
        /// <value>
        /// The current approver.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverDetails(true, DCRDCNListNames.DCNAPPAPPROVALMATRIX)]
        public ApplicationStatus CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCNAPPAPPROVALMATRIX)]
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
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SendBackTo { get; set; }

        /// <summary>
        /// Gets or sets the Approve or Rework.
        /// </summary>
        /// <value>
        /// The Approve or Rework.
        /// </value>
        [DataMember, Required, FieldColumnName("DesignEngineerApproveRework")]
        public string ApproveRework { get; set; }

        /// <summary>
        /// Gets or sets the Vendor DCN.
        /// </summary>
        /// <value>
        /// The Vendor DCN.
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, DCRDCNListNames.VENDORDCNLIST, typeof(VendorDCN))]
        public List<ITrans> VendorDCNList { get; set; }

        /// <summary>
        /// Gets or sets the Vendor DCN.
        /// </summary>
        /// <value>
        /// The Vendor DCN.
        /// </value>
        [DataMember, IsListColumn(false)]
        public List<VendorDCN> VendorDCNPostList { get; set; }

        /// <summary>
        /// Gets or sets DCRProcessICommentr.
        /// </summary>
        /// <value>
        /// The DCRProcessIComment.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DCRProcessIComment { get; set; }

        /// <summary>
        /// SCM Comment
        /// </summary>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string SCMComment { get; set; }

        /// <summary>
        /// Gets or sets DesignEngComment.
        /// </summary>
        /// <value>
        /// The DesignEngComment.
        /// </value>
        [DataMember, Required, IsListColumn(false), MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DesignEngComment { get; set; }

        /// <summary>
        /// QA Date of Approval
        /// </summary>
        [DataMember]
        public DateTime? QADateofApproval { get; set; }
    }
}
