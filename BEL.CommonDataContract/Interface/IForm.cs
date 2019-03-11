namespace BEL.CommonDataContract
{
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// IForm Interface
    /// </summary>
    public interface IForm
    {
        /// <summary>
        /// Gets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        string FormName { get; }

        /// <summary>
        /// Gets or sets the form status.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        string FormStatus { get; set; }

        /// <summary>
        /// Gets or sets the form approval level.
        /// </summary>
        /// <value>
        /// The form approval level.
        /// </value>
        int FormApprovalLevel { get; set; }

        /// <summary>
        /// Gets or sets the total approval required.
        /// </summary>
        /// <value>
        /// The total approval required.
        /// </value>
        int TotalApprovalRequired { get; set; }

        /// <summary>
        /// Gets or sets the sections list.
        /// </summary>
        /// <value>
        /// The sections list.
        /// </value>
        List<ISection> SectionsList { get; set; }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        List<Button> Buttons { get; set; }

        /// <summary>
        /// Gets or sets the name of the approval matrix list.
        /// </summary>
        /// <value>
        /// The name of the approval matrix list.
        /// </value>
        string ApprovalMatrixListName { get; set; }

        /// <summary>
        /// Gets or sets the name of the main list.
        /// </summary>
        /// <value>
        /// The name of the main list.
        /// </value>
        string MainListName { get; set; }
    }
}
