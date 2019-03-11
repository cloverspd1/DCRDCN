﻿namespace BEL.DCRDCNWorkflow.Models.Master
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
    public class DCRProcessInchargeMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [DataMember, FieldColumnName("Title")]
        public string Title { get; set; }
       
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [FieldColumnName("Title")]
        [DataMember]
        public string Value { get; set; }
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>        
        [DataMember, FieldColumnName("BusinessUnit", true, false, "Title")]
        public string BusinessUnit { get; set; }
    }
}