namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Field Belong To List Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class FieldBelongToListAttribute : Attribute
    {
        /// <summary>
        /// The list namestr
        /// </summary>
        private string listNamestr = string.Empty;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldBelongToListAttribute"/> class.
        /// </summary>
        /// <param name="listNames">The list names.</param>
        public FieldBelongToListAttribute(string listNames)
        {
            this.listNamestr = listNames;
        }
        
        /// <summary>
        /// Gets the list names.
        /// </summary>
        /// <value>
        /// The list names.
        /// </value>
        public string ListNames
        {
            get
            {
                return this.listNamestr;
            }
        }
    }
}
