namespace BEL.DCRDCNWorkflow.Models.DCN
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// DR Form
    /// </summary>
    [DataContract, Serializable]
    public class DCNForm : IForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DRForm"/> class.
        /// </summary>
        public DCNForm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DRForm"/> class.
        /// </summary>
        /// <param name="setSections">if set to <c>true</c> [set sections].</param>
        public DCNForm(bool setSections)
        {
            if (setSections)
            {
                this.ApprovalMatrixListName = DCRDCNListNames.DCNAPPAPPROVALMATRIX;
                this.SectionsList = new List<ISection>();
                this.SectionsList.Add(new DesignEngineerSection(true));
                this.SectionsList.Add(new DesignEngineerInchargeSection(true));
                this.SectionsList.Add(new DCNQAInchargeSection(true));
                this.SectionsList.Add(new DCNSCMSection(true)); 
                this.SectionsList.Add(new DCRProcessInchargeSection(true));
                this.SectionsList.Add(new ApplicationStatusSection(true) { SectionName = SectionNameConstant.APPLICATIONSTATUS });
                this.SectionsList.Add(new ActivityLogSection(DCRDCNListNames.DCNACTIVITYLOG));
                this.Buttons = new List<Button>();
                this.MainListName = DCRDCNListNames.DCNLIST;
            }
        }

        /// <summary>
        /// Gets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        [DataMember]
        public string FormName
        {
            get { return FormNames.DCNFORM; }
        }

        /// <summary>
        /// Gets or sets the form status.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string FormStatus { get; set; }

        /// <summary>
        /// Gets or sets the form approval level.
        /// </summary>
        /// <value>
        /// The form approval level.
        /// </value>
        [DataMember]
        public int FormApprovalLevel { get; set; }

        /// <summary>
        /// Gets or sets the total approval required.
        /// </summary>
        /// <value>
        /// The total approval required.
        /// </value>
        [DataMember]
        public int TotalApprovalRequired { get; set; }

        /// <summary>
        /// Gets or sets the sections list.
        /// </summary>
        /// <value>
        /// The sections list.
        /// </value>
        [DataMember]
        public List<ISection> SectionsList { get; set; }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        [DataMember]
        public List<Button> Buttons { get; set; }

        /// <summary>
        /// Gets or sets the name of the approval matrix list.
        /// </summary>
        /// <value>
        /// The name of the approval matrix list.
        /// </value>
        [DataMember]
        public string ApprovalMatrixListName { get; set; }

        /// <summary>
        /// Gets or sets the name of the main list.
        /// </summary>
        /// <value>
        /// The name of the main list.
        /// </value>
        [DataMember]
        public string MainListName { get; set; }
    }
}
