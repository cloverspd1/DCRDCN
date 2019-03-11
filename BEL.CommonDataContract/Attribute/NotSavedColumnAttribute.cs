namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Not Saved column attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NotSavedColumnAttribute : Attribute
    {
        /// <summary>
        /// The not Saved Column
        /// </summary>
        private bool notSavedColumn = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSavedAttribute"/> class.
        /// </summary>
        /// <param name="isListColumn">if set to <c>true</c> [not Saved Column].</param>
        public NotSavedColumnAttribute(bool notSavedColumn)
        {
            this.notSavedColumn = notSavedColumn;
        }

        /// <summary>
        /// Gets a value indicating whether this instance not Saved Column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is list column; otherwise, <c>false</c>.
        /// </value>
        public bool NotSavedColumn
        {
            get
            {
                return this.notSavedColumn;
            }
        }
    }
}
