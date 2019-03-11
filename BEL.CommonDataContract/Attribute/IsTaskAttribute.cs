namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Is Task List Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsTaskAttribute : Attribute
    {
        /// <summary>
        /// The _task field
        /// </summary>
        private bool taskField = true;

        /// <summary>
        /// The task list name
        /// </summary>
        private string taskListName = string.Empty;

        /// <summary>
        /// The task type
        /// </summary>
        private Type taskType = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsTaskAttribute"/> class.
        /// </summary>
        /// <param name="taskField">if set to <c>true</c> [task field].</param>
        /// <param name="taskListName">Name of the task list.</param>
        /// <param name="taskType">Type of the task.</param>
        public IsTaskAttribute(bool taskField, string taskListName, Type taskType)
        {
            this.taskField = taskField;
            this.taskListName = taskListName;
            this.taskType = taskType;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is task field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is task field; otherwise, <c>false</c>.
        /// </value>
        public bool IsTaskField
        {
            get
            {
                return this.taskField;
            }
        }

        /// <summary>
        /// Gets the name of the task list.
        /// </summary>
        /// <value>
        /// The name of the task list.
        /// </value>
        public string TaskListName
        {
            get
            {
                return this.taskListName;
            }
        }

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        /// <value>
        /// The type of the task.
        /// </value>
        public Type TaskType
        {
            get
            {
                return this.taskType;
            }
        }
    }
}
