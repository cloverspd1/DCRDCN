namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Is Primary Key Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsPrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// <summary>
        /// The _is file
        /// </summary>
        /// The _is file
        /// </summary>
        private bool isPrimaryKey = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsPrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="isPrimaryKey">if set to <c>true</c> [is primary key].</param>
        public IsPrimaryKeyAttribute(bool isPrimaryKey)
        {
            this.isPrimaryKey = isPrimaryKey;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is primary key.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is primary key; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrimaryKey
        {
            get
            {
                return this.isPrimaryKey;
            }
        }
    }
}
