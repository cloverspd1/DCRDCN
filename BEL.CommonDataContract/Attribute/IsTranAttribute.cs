namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Is Tran Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsTranAttribute : Attribute
    {
        /// <summary>
        /// The tran field
        /// </summary>
        private bool tranField = true;

        /// <summary>
        /// The tran list name
        /// </summary>
        private string tranListName = string.Empty;

        /// <summary>
        /// The tran type
        /// </summary>
        private Type tranType = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsTranAttribute"/> class.
        /// </summary>
        /// <param name="taskField">if set to <c>true</c> [task field].</param>
        /// <param name="taskListName">Name of the task list.</param>
        /// <param name="taskType">Type of the task.</param>
        public IsTranAttribute(bool taskField, string taskListName, Type taskType)
        {
            this.tranField = taskField;
            this.tranListName = taskListName;
            this.tranType = taskType;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tran field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tran field; otherwise, <c>false</c>.
        /// </value>
        public bool IsTranField
        {
            get
            {
                return this.tranField;
            }
        }

        /// <summary>
        /// Gets the name of the tran list.
        /// </summary>
        /// <value>
        /// The name of the tran list.
        /// </value>
        public string TranListName
        {
            get
            {
                return this.tranListName;
            }
        }

        /// <summary>
        /// Gets the type of the tran.
        /// </summary>
        /// <value>
        /// The type of the tran.
        /// </value>
        public Type TranType
        {
            get
            {
                return this.tranType;
            }
        }
    }
}
