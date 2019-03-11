namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// <summary>
    /// Action Status
    /// </summary>
    /// Action Status
    /// </summary>
    [DataContract, Serializable]
    public class ActionStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionStatus"/> class.
        /// </summary>
        public ActionStatus()
        {
            this.Messages = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is succeed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is succeed; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsSucceed { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [DataMember]
        public List<string> Messages { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        [DataMember]
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the extra data.
        /// </summary>
        /// <value>
        /// The extra data.
        /// </value>
        [DataMember]
        public string ExtraData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is file; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsFile { get; set; }
    }
}
