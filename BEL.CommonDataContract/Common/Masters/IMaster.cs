namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// IMaster Interface
    /// </summary>
    public interface IMaster
    {
        /// <summary>
        /// Gets or sets the name of master.
        /// </summary>
        /// <value>
        /// The name of master.
        /// </value>
        string NameOfMaster { get; set; }

        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        /// <value>
        /// The name of the list.
        /// </value>
        string ListName { get; set; }

        /// <summary>
        /// Gets or sets the caching interval in HRS.
        /// </summary>
        /// <value>
        /// The caching interval in HRS.
        /// </value>
        int CachingIntervalInHrs { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        string Scope { get; set; }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>
        /// The type of the item.
        /// </value>
        Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        List<IMasterItem> Items { get; set; }
    }
}
