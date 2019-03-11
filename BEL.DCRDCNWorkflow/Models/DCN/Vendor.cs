namespace BEL.DCRDCNWorkflow.Models.DCN
{
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Vendor
    /// </summary>
    [DataContract, Serializable]
    public class Vendor : ITrans
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Name.
        /// </summary>
        /// <value>
        /// The Vendor Name.
        /// </value>
        [DataMember,FieldColumnName("Title")]                                    //Is this field Lookup or not?
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        /// <value>
        /// The Start Quantity.
        /// </value>
        [DataMember]                                //Required or not?
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets the FG Stock.
        /// </summary>
        /// <value>
        /// The Start FG Stock.
        /// </value>
        [DataMember]
        public double? FGStock { get; set; }

        /// <summary>
        /// Gets or sets the Existing Component Stock.
        /// </summary>
        /// <value>
        /// The Start Existing Component Stock.
        /// </value>
        [DataMember]
        public double? ExistingComponentStock { get; set; }

        /// <summary>
        /// Gets or sets the Implementation Date.
        /// </summary>
        /// <value>
        /// The Implementation Date.
        /// </value>
        [DataMember]         //Required or not?
        public DateTime? DateOfImplementation { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [DataMember, FieldColumnName("RequestID", true, false, "ID")]
        public int RequestID { get; set; }

        /// <summary>
        /// Gets or sets the Request By.
        /// </summary>
        /// <value>
        /// The Request By.
        /// </value>
        [DataMember, IsPerson(true, false),IsViewer]
        public string RequestBy { get; set; }

        /// <summary>
        /// Gets or sets the Request Date.
        /// </summary>
        /// <value>
        /// The Request Date.
        /// </value>
        [DataMember]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the item action.
        /// </summary>
        /// <value>
        /// The item action.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ItemActionStatus ItemAction { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }
    }
}