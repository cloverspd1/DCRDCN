namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Approver Matrix Field Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsApproverMatrixFieldAttribute : Attribute
    {
        /// <summary>
        /// The is approver matrix field
        /// </summary>
        private bool isApproverMatrixField = true;

        /// <summary>
        /// The approver matrix list name
        /// </summary>
        private string approverMatrixListName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsApproverMatrixFieldAttribute"/> class.
        /// </summary>
        /// <param name="isApproverMatrixField">if set to <c>true</c> [is approver matrix field].</param>
        /// <param name="approverMatrixListName">Name of the approver matrix list.</param>
        public IsApproverMatrixFieldAttribute(bool isApproverMatrixField, string approverMatrixListName)
        {
            this.isApproverMatrixField = isApproverMatrixField;
            this.approverMatrixListName = approverMatrixListName;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is approver matrix field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is approver matrix field; otherwise, <c>false</c>.
        /// </value>
        public bool IsApproverMatrixField
        {
            get
            {
                return this.isApproverMatrixField;
            }
        }

        /// <summary>
        /// Gets the name of the approver matrix list.
        /// </summary>
        /// <value>
        /// The name of the approver matrix list.
        /// </value>
        public string ApproverMatrixListName
        {
            get
            {
                return this.approverMatrixListName;
            }
        }
    }
}
