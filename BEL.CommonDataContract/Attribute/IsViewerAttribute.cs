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
    public class IsViewerAttribute : Attribute
    {
        /// <summary>
        /// The is list column
        /// </summary>
        private bool isViewer = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsViewerAttribute"/> class.
        /// </summary>
        /// <param name="isViewerColumn">if set to <c>true</c> [is viewer column].</param>
        public IsViewerAttribute(bool isViewerColumn = true)
        {
            this.isViewer = isViewerColumn;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is list column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is list column; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewer
        {
            get
            {
                return this.isViewer;
            }
        }
    }
}
