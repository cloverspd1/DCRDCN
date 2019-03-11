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
    public class EmployeeMasterListItem : IMasterItem
    {
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [DataMember, FieldColumnName("Title")]
        public string EmployeeName { get; set; }

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
        /// Gets or sets the Alias.
        /// </summary>
        /// <value>
        /// The Alias.
        /// </value>
        [DataMember, IsPerson(true, false)]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        /// <value>
        /// The Email.
        /// </value>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Company Name.
        /// </summary>
        /// <value>
        /// The Company Name.
        /// </value>
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the Employee Code.
        /// </summary>
        /// <value>
        /// The Employee Code.
        /// </value>
        [DataMember]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets the Workspace.
        /// </summary>
        /// <value>
        /// The Workspace.
        /// </value>
        [DataMember]
        public string Workspace { get; set; }

        /// <summary>
        /// Gets or sets the Contact No.
        /// </summary>
        /// <value>
        /// The Contact No.
        /// </value>
        [DataMember]
        public string ContactNo { get; set; }

        /// <summary>
        /// Gets or sets the Designation.
        /// </summary>
        /// <value>
        /// The Designation.
        /// </value>
        [DataMember]
        public string Designation { get; set; }

        /// <summary>
        /// Gets or sets the Reporting Manager.
        /// </summary>
        /// <value>
        /// The Reporting Manager.
        /// </value>
        [DataMember, IsPerson(true, false)]
        public string ReportingManager { get; set; }

        /// <summary>
        /// Gets or sets the Department.
        /// </summary>
        /// <value>
        /// The Department.
        /// </value>
        [DataMember]
        public string Department { get; set; }
    }
}