namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Is Append List column attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsAppendFieldAttribute : Attribute
    {
        /// <summary>
        /// The is append field
        /// </summary>
        private bool isAppendField = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsAppendFieldAttribute"/> class.
        /// </summary>
        /// <param name="isAppendFieldColumn">if set to <c>true</c> [is append field column].</param>
        public IsAppendFieldAttribute(bool isAppendFieldColumn = true)
        {
            this.isAppendField = isAppendFieldColumn;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is append field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is append field; otherwise, <c>false</c>.
        /// </value>
        public bool IsAppendField
        {
            get
            {
                return this.isAppendField;
            }
        }
    }
}
