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
    public class SCMSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SCMSection"/> class.
        /// </summary>
        public SCMSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCMSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public SCMSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(DCRDCNListNames.DCRLIST, true) };
                this.SectionName = DCRSectionName.SCMSECTION;
                this.VendorDCRList = new List<ITrans>();
                this.ApproversList = new List<ApplicationStatus>();
                this.CurrentApprover = new ApplicationStatus();
                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new ApproverMaster());
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
        [DataMember, Required]
        public string FeedbackFromSCM { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, DCRDCNListNames.DCRAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the visit location.
        /// </summary>
        /// <value>
        /// The visit location.
        /// </value>
        [DataMember]
        public string BusinessUnit { get; set; }
    }
}
