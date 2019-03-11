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
    /// Specification Grid
    /// </summary>
    [DataContract, Serializable]
    public class Specification : ITrans
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
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [DataMember, FieldColumnName("RequestID", true ,false,"ID")]
        public int RequestID { get; set; }

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
        /// Gets or sets the request by.
        /// </summary>
        /// <value>
        /// The request by.
        /// </value>
        [DataMember, IsPerson(true, false),IsViewer]
        public string RequestBy { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the Component.
        /// </summary>
        /// <value>
        /// The Componenet
        /// </value>
        [DataMember, Required, FieldColumnName("Component")]
        public string ComponentRawMaterial { get; set; }

        /// <summary>
        /// Gets or sets the Present Specification.
        /// </summary>
        /// <value>
        /// The Present Specification
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string PresentSpecification { get; set; }

        /// <summary>
        /// Gets or sets the Revised Specification.
        /// </summary>
        /// <value>
        /// The Revised Specification
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string RevisedSpecification { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status
        /// </value>
        [DataMember]
        public string Status { get; set; }

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
        [DataMember]
        public string FNLPresentSpecification { get; set; }
        
        /// <summary>
        /// FNL Revised Specification File
        /// </summary>
        [DataMember]
        public string FNLRevisedSpecification { get; set; }
    }
}