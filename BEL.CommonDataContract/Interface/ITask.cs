namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ITask interface
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        int RequestID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the target date.
        /// </summary>
        /// <value>
        /// The target date.
        /// </value>
        DateTime? TargetDate { get; set; }

        /// <summary>
        /// Gets or sets the actual start date.
        /// </summary>
        /// <value>
        /// The actual start date.
        /// </value>
        DateTime? ActualStartDate { get; set; }

        /// <summary>
        /// Gets or sets the actual end date.
        /// </summary>
        /// <value>
        /// The actual end date.
        /// </value>
        DateTime? ActualEndDate { get; set; }

        /// <summary>
        /// Gets or sets the action by.
        /// </summary>
        /// <value>
        /// The action by.
        /// </value>
        string ActionBy { get; set; }

        /// <summary>
        /// Gets or sets the task action.
        /// </summary>
        /// <value>
        /// The task action.
        /// </value>
        TaskActionStatus TaskAction { get; set; }

        /// <summary>
        /// Gets or sets the typeof task.
        /// </summary>
        /// <value>
        /// The typeof task.
        /// </value>
        string TypeofTask { get; set; }

        /// <summary>
        /// Gets or sets the forward to.
        /// </summary>
        /// <value>
        /// The forward to.
        /// </value>
        string ForwardTo { get; set; }

        /// <summary>
        /// Gets or sets the forward to.
        /// </summary>
        /// <value>
        /// The forward to.
        /// </value>
        string Status { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        string Comment { get; set; }

        /// <summary>
        /// Gets or sets the comment history.
        /// </summary>
        /// <value>
        /// The comment history.
        /// </value>
        string CommentHistory { get; set; }
    }
}
