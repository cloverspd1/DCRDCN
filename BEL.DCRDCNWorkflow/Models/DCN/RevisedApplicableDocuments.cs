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
    /// Revised Applicable Doc
    /// </summary>
    [DataContract, Serializable]
    public class RevisedApplicableDocuments : ITrans
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
        /// Gets or sets the Doc No.
        /// </summary>
        /// <value>
        /// The Doc No.
        /// </value>
        [DataMember, Required, FieldColumnName("DocumentNumber")]
        public string DocumentNo { get; set; }

        /// <summary>
        /// Gets or sets the Doc Title.
        /// </summary>
        /// <value>
        /// The Doc Title.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string DocumentTitle { get; set; }

        /// <summary>
        /// Gets or sets the Revision No.
        /// </summary>
        /// <value>
        /// The Revision No.
        /// </value>
        [DataMember, Required, MaxLength(255, ErrorMessage = "this field should not be more then 255 characters")]
        public string RevisionNo { get; set; }

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
        [DataMember, IsListColumn(false)]
        public string FileNameList { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [DataMember, FieldColumnName("RequestID", true,false,"ID")]
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
        [DataMember, DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

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
    }
}