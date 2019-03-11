namespace BEL.DCRDCNWorkflow.Models.DCN
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.Models.Common;

    /// <summary>
    /// DCR No 
    /// </summary>
    [DataContract, Serializable]
    public class DCRDetails
    {
        /// <summary>
        /// Gets or sets the Vendor DCR.
        /// </summary>
        /// <value>
        /// The Vendor DCR
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, DCRDCNListNames.VENDORDCNLIST, typeof(VendorDCN))]
        public List<ITrans> VendorDCRList { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string DCRNo { get; set; }

        /// <summary>
        /// Gets or sets the dcrid.
        /// </summary>
        /// <value>
        /// The dcrid.
        /// </value>
        [DataMember]
        public int DCRID { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string DCNNo { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public int ID { get; set; }

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
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
        public string QAIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("QAIncharge"), IsViewer]
        public string QAInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, true), IsListColumn(false), IsViewer]
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
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, true), IsListColumn(false), IsViewer]
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
        [DataMember, IsPerson(true,false), IsListColumn(false), IsViewer]
        public string FinalDesignEngineer { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("FinalDesignEngineer"), IsViewer]
        public string FinalDesignEngineerName { get; set; }

        /// <summary>
        /// Gets or sets the design engineer incharge.
        /// </summary>
        /// <value>
        /// The design engineer incharge.
        /// </value>
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
        public string DesignEngineerIncharge { get; set; }

        /// <summary>
        /// Gets or sets the name of the design engineer incharge.
        /// </summary>
        /// <value>
        /// The name of the design engineer incharge.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("DesignEngineerIncharge"), IsViewer]
        public string DesignEngineerInchargeName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember]
        public string RequestDepartment { get; set; }

        /// <summary>
        /// Gets or sets the Product Name.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember]
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
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
        [DataMember, IsPerson(true, false), IsListColumn(false), IsViewer]
        public string LUMMarketingIncharge { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("LUMMarketingIncharge"), IsViewer]
        public string LUMMarketingInchargeName { get; set; }
    }
}