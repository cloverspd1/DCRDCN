namespace BEL.CommonDataContract
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// File Details
    /// </summary>
    [Serializable]
    public class FileDetails
    {
        /// <summary>
        /// Gets or sets the file identifier.
        /// </summary>
        /// <value>
        /// The file identifier.
        /// </value>
        [DataMember]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        /// <value>
        /// The content of the file.
        /// </value>
        [DataMember]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// Gets or sets the file URL.
        /// </summary>
        /// <value>
        /// The file URL.
        /// </value>
        [DataMember]
        public string FileURL { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public FileStatus Status { get; set; }
    }

    /// <summary>
    /// File Status
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// The no action
        /// </summary>
        NoAction = 0,

        /// <summary>
        /// The new
        /// </summary>
        New = 1,

        /// <summary>
        /// The delete
        /// </summary>
        Delete = 2,
    }
}