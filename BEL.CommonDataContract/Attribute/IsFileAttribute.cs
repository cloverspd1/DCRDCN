namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Is File Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    
    /// <summary>
    /// Is File Attribute
    /// </summary>
    public class IsFileAttribute : Attribute
    {
        /// <summary>
        /// <summary>
        /// The _is file
        /// </summary>
        /// The _is file
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="IsFileAttribute"/> class.
        /// </summary>
        /// <param name="isFile">if set to <c>true</c> [is file].</param>
        private bool isFile1 = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsFileAttribute"/> class.
        /// <summary>
        /// Gets a value indicating whether this instance is file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is file; otherwise, <c>false</c>.
        /// </value>
        /// </summary>
        /// <param name="isFile">if set to <c>true</c> [is file].</param>
        public IsFileAttribute(bool isFile)
        {
            this.isFile1 = isFile;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is file; otherwise, <c>false</c>.
        /// </value>
        public bool IsFile
        {
            get
            {
                return this.isFile1;
            }
        }
    }
}
