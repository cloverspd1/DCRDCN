namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Master Data Item
    /// </summary>
    [DataContract, Serializable]
    public class MasterDataItem : IMasterItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="title">The title.</param>
        public MasterDataItem(string value, string title)
        {
            this.Value = value;
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public string Value { get; set; }
    }
}
