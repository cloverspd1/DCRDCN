namespace BEL.CommonDataContract
{
    using Microsoft.SharePoint.Client;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// User Details
    /// </summary>  
    [DataContract, Serializable]
    public class UserDetails
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        /// <value>
        /// The user email.
        /// </value>
        [DataMember]
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the type of the role.
        /// </summary>
        /// <value>
        /// The type of the role.
        /// </value>
        [DataMember]
        public string RoleType { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        [DataMember]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [DataMember]
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the employee code.
        /// </summary>
        /// <value>
        /// The employee code.
        /// </value>
        [DataMember]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets the designation.
        /// </summary>
        /// <value>
        /// The designation.
        /// </value>
        [DataMember]
        public string Designation { get; set; }

        /// <summary>
        /// Gets or sets the workspace.
        /// </summary>
        /// <value>
        /// The workspace.
        /// </value>
        [DataMember]
        public string Workspace { get; set; }

        /// <summary>
        /// Gets or sets the contact no.
        /// </summary>
        /// <value>
        /// The contact no.
        /// </value>
        [DataMember]
        public string ContactNo { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [DataMember]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the sub section.
        /// </summary>
        /// <value>
        /// The sub section.
        /// </value>
        [DataMember]
        public string SubSection { get; set; }

        /// <summary>
        /// Gets or sets the upward reporting authorities.
        /// </summary>
        /// <value>
        /// The upward reporting authorities.
        /// </value>
        [DataMember]
        public string ReportingManager { get; set; }

        ///// <summary>
        ///// Current SP User
        ///// </summary>
        //[DataMember, Serializable]
        //public User CurrentSPUser { get; set; }

        /// <summary>
        /// Current SP User
        /// </summary>
        [DataMember]
        public string LoginName { get; set; }
    }
}
