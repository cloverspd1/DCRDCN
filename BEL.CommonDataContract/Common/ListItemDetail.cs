namespace BEL.CommonDataContract
{
    using Microsoft.SharePoint.Client;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// List Item detaisl
    /// </summary>
    [DataContract, Serializable]
    public class ListItemDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemDetail"/> class.
        /// </summary>
        public ListItemDetail()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemDetail"/> class.
        /// </summary>
        /// <param name="listname">The listname.</param>
        /// <param name="isMainList">if set to <c>true</c> [is main list].</param>
        public ListItemDetail(string listname, bool isMainList)
        {
            this.IsMainList = isMainList;
            this.ListName = listname;
            this.ListItemObject = null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemDetail"/> class.
        /// </summary>
        /// <param name="listname">The listname.</param>
        /// <param name="isMainList">if set to <c>true</c> [is main list].</param>
        public ListItemDetail(string listname, bool isMainList,string camlQuery)
        {
            this.IsMainList = isMainList;
            this.ListName = listname;
            this.ListItemObject = null;
            this.CamlQuery = camlQuery;
        }

        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        [DataMember]
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        /// <value>
        /// The name of the list.
        /// </value>
        [DataMember]
        public string ListName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is main list.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is main list; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsMainList { get; set; }

        /// <summary>
        /// Gets or sets the list item object.
        /// </summary>
        /// <value>
        /// The list item object.
        /// </value>
        [DataMember]
        public ListItem ListItemObject { get; set; }

        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        /// <value>
        /// The name of the list.
        /// </value>
        [DataMember]
        public string CamlQuery { get; set; }
    }
}
