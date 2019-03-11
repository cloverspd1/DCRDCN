namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Approvers Class
    /// </summary>
    public class Approvers
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic approve.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is automatic approve; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoApprove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is escalate.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is escalate; otherwise, <c>false</c>.
        /// </value>
        public bool IsEscalate { get; set; }

        /// <summary>
        /// Gets or sets the automatic approval days.
        /// </summary>
        /// <value>
        /// The automatic approval days.
        /// </value>
        public int AutoApprovalDays { get; set; }

        /// <summary>
        /// Gets or sets the escalation days.
        /// </summary>
        /// <value>
        /// The escalation days.
        /// </value>
        public int EscalationDays { get; set; }
    }
}
