namespace BEL.DCRDCNWorkflow.Models.Master
{
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Department Master List Item
    /// </summary>
    [DataContract, Serializable]
    public class DivisionMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>        
        [DataMember, FieldColumnName("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>        
        [DataMember, FieldColumnName("Title")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>        
        [DataMember, FieldColumnName("BusinessUnit", true, false, "Title")]
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public string DivisionValue { get; set; }

        /// <summary>
        /// Gets or sets the Marketing Incharge Person.
        /// </summary>
        /// <value>
        /// The Marketing Incharge Person.
        /// </value>
        [DataMember]
        public string MarketingGroup { get; set; }
    }
}