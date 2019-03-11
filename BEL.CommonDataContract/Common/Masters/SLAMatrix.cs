namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// SLA Matrix class
    /// </summary>
    [DataContract, Serializable]
    public class SLAMatrix
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [DataMember]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets the req identifier.
        /// </summary>
        /// <value>
        /// The req identifier.
        /// </value>
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Levels { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [DataMember]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the fill by role.
        /// </summary>
        /// <value>
        /// The fill by role.
        /// </value>
        [DataMember]
        public string FillByRole { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        public string Division { get; set; }

        /// <summary>
        /// Gets or sets the sub division.
        /// </summary>
        /// <value>
        /// The sub division.
        /// </value>
        public string SubDivision { get; set; }

        /// <summary>
        /// Gets or sets the SLA Days.
        /// </summary>
        /// <value>
        /// The sub division.
        /// </value>
        public int Days { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic approval.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is automatic approval; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsAutoApproval { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is optional; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsOptional { get; set; }
    }
}