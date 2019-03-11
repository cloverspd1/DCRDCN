namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The DivisionAdminDetails
    /// </summary>
    [DataContract, Serializable]
    public class DivisionAdminDetails
    {
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
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets the DivisionAdmins.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string DivisionAdmins { get; set; }

        /// <summary>
        /// Gets or sets the DivisionCodes.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string DivisionCodes { get; set; }

        /// <summary>
        /// Gets or sets the DivisionFieldName.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string DivisionFieldName { get; set; }
    }
}
