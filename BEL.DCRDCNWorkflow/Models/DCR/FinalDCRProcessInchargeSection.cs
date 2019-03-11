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
    public class FinalDCRProcessInchargeSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountAdminSection"/> class.
        /// </summary>
        public FinalDCRProcessInchargeSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountAdminSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public FinalDCRProcessInchargeSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true) };
                this.SectionName = DCRSectionName.FINALDCRPROCESSINCHARGESECTION;
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

        ///// <summary>
        ///// Gets or sets the DDHOD Comments.
        ///// </summary>
        ///// <value>
        ///// The DDHOD Comments.
        ///// </value>
        //[DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 character")]
        //public string CommentsAfterProtoTesting { get; set; }

        /// <summary>
        /// Gets or sets the DCRDCN Incharge Comments.
        /// </summary>
        /// <value>
        /// The DCRDCN Incharge Comments.
        /// </value>
        [DataMember]
        public string RejectedComment { get; set; }

        /// <summary>
        /// Gets or sets the Is DCR approved.
        /// </summary>
        /// <value>
        /// The Is DCR approved.
        /// </value>
        [DataMember, Required]
        public string IsApproved { get; set; }

        ///// <summary>
        ///// Gets or sets the Design Document Engineer.
        ///// </summary>
        ///// <value>
        ///// The Design Document Engineer.
        ///// </value>
        //[DataMember, Required,IsPerson(true,false,false),IsViewer]
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
        //[DataMember, Required, IsPerson(true, false, true), IsViewer, FieldColumnName("DesignDocumentEngineer")]
        //public string DesignDocumentEngineerName { get; set; }   

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

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
        /// Gets or sets the target date of implementation.
        /// </summary>
        /// <value>
        /// The target date of implementation.
        /// </value>
        [DataMember]
        public DateTime? TargetDateOfImplementation { get; set; }
    }
}
