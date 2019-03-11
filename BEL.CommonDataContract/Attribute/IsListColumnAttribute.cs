namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Is List column attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsListColumnAttribute : Attribute
    {
        /// <summary>
        /// The is list column
        /// </summary>
        private bool isListColumn = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsListColumnAttribute"/> class.
        /// </summary>
        /// <param name="isListColumn">if set to <c>true</c> [is list column].</param>
        public IsListColumnAttribute(bool isListColumn)
        {
            this.isListColumn = isListColumn;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is list column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is list column; otherwise, <c>false</c>.
        /// </value>
        public bool IsListColumn
        {
            get
            {
                return this.isListColumn;
            }
        }
    }
}
