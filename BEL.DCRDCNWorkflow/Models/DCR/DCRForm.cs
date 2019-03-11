namespace BEL.DCRDCNWorkflow.Models.DCR
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;
    using BEL.DCRDCNWorkflow.Models.DCR;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// DR Form
    /// </summary>
    [DataContract, Serializable]
    public class DCRForm : IForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DRForm"/> class.
        /// </summary>
        public DCRForm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DRForm"/> class.
        /// </summary>
        /// <param name="setSections">if set to <c>true</c> [set sections].</param>
        public DCRForm(bool setSections)
        {
            if (setSections)
            {
                this.ApprovalMatrixListName = DCRDCNListNames.DCRAPPAPPROVALMATRIX;
                this.SectionsList = new List<ISection>();
                this.SectionsList.Add(new DCRDetailSection(true));  ////LEVEL 0
                this.SectionsList.Add(new DCRInchargeNavigatorSection(true));        ////LEVEL 1
                this.SectionsList.Add(new DCRProcessInchargeSection(true));          ////LEVEL 2
                this.SectionsList.Add(new DesignEngineerInchargeSection(true));      ////LEVEL 3
                this.SectionsList.Add(new DesignEngineerSection(true));              ////LEVEL 4
                this.SectionsList.Add(new QAInchargeSection(true));                  ////LEVEL 3
                this.SectionsList.Add(new SCMInchargeSection(true));                 ////LEVEL 3
                this.SectionsList.Add(new QASection(true));                         //// LEVEL 4
                this.SectionsList.Add(new SCMSection(true));                        //// LEVEL 4
                this.SectionsList.Add(new KAPMarketingInchargeSection(true));       //// LEVEL 5
                this.SectionsList.Add(new KAPMarketingSection(true));               //// LEVEL 6

                this.SectionsList.Add(new DAPMarketingInchargeSection(true));       //// LEVEL 5
                this.SectionsList.Add(new DAPMarketingSection(true));               //// LEVEL 6

                this.SectionsList.Add(new FANSMarketingInchargeSection(true));      //// LEVEL 5
                this.SectionsList.Add(new FANSMarketingSection(true));              //// LEVEL 6    

                this.SectionsList.Add(new LightingMarketingInchargeSection(true));  //// LEVEL 5
                this.SectionsList.Add(new LightingMarketingSection(true));          //// LEVEL 6

                this.SectionsList.Add(new MorphyRechardMarketingInchargeSection(true)); //// LEVEL 5
                this.SectionsList.Add(new MorphyRechardMarketingSection(true));         //// LEVEL 6

                this.SectionsList.Add(new LUMMarketingInchargeSection(true));       //// LEVEL 5
                this.SectionsList.Add(new LUMMarketingSection(true));               //// LEVEL 6

                this.SectionsList.Add(new CCInchargeSection(true));                 //// LEVEL 5

                this.SectionsList.Add(new CostingSection(true));                    //// LEVEL 5

                this.SectionsList.Add(new FinalDCRProcessInchargeSection(true));   //// LEVEL 7
                this.SectionsList.Add(new ApplicationStatusSection(true) { SectionName = SectionNameConstant.APPLICATIONSTATUS });
                this.SectionsList.Add(new ActivityLogSection(DCRDCNListNames.DCRACTIVITYLOG));
                this.Buttons = new List<Button>();
                this.MainListName = DCRDCNListNames.DCRLIST;
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
            get { return FormNames.DCRFORM; }
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
